using Confluent.Kafka;
using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Coding_Exercise.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Coding_Exercise.Configuration;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka.SyncOverAsync;
using JsonSerDes;


public class WeatherConsumer : IHostedService
{
    //readonly configuration can only be set in constructor. Prevents alterations once application begins
    private readonly string _GroupId;
    private readonly string _BootstrapServers;
    private readonly string _Topic;
    private readonly string _Forward;
    public WeatherConsumer(IOptions<KafkaConfig> options)
    {
        _GroupId = options.Value.GroupID;
        _BootstrapServers = options.Value.BootstrapServers;
        _Topic = options.Value.Topic;
        _Forward = options.Value.ForwardEndpoint; 
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {


        var config = new ConsumerConfig
        {
            GroupId = _GroupId,
            BootstrapServers = _BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            
        };

        try
        {

            using (IConsumer<string, string> consumerBuilder = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((k,i)=>
            Debug.WriteLine("Consumer Error: " + i.Reason))
            .Build())
            {
                //attach subscription to kafka server for requested topic
                consumerBuilder.Subscribe(_Topic);
                var cancelToken = new CancellationTokenSource();
          
                try
                {
                    while (true)
                    {

                        var consumer = consumerBuilder.Consume(cancelToken.Token);
                        WeatherRequestData? wrd = null;
                        try
                        {     
                             wrd = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherRequestData>(consumer.Message.Value);    
                        }
                        catch(Exception jsonException)
                        {
                            Debug.WriteLine($"Error:{jsonException.Message}");
                        }
                       
                        if (wrd != null)
                        {
                            Debug.WriteLine($"Weather Record Received:{ wrd.Date}");
                            //Process Object
                            //Task seems specific to these paired objects. Opportunities here for heavier/more complex mapping
                            //Code would/should likely live elsewhere depending on functionality desired.
                            var data = ProcessNotification(wrd);

                            //don't await as there is no manageable interaction or reason to prevent additional subscription notifications to process.
                            GenerateForwardRequest(data);
                        } 
                    }
                }
                //potential chance for recovery depending on issue. TBD based on required functionality. 
                catch (OperationCanceledException)
                {
                    consumerBuilder.Close();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        return Task.CompletedTask;
    }

    private WeatherData ProcessNotification(WeatherRequestData weatherNotification)
    {
        return new WeatherData
            {
                date = weatherNotification.Date,
                humidity = weatherNotification.Humidity,
                pressure = weatherNotification.Pressure,
                rain = weatherNotification.Rain,
                temperature = weatherNotification.Temperature,
                wind = weatherNotification.Wind,
                windDirection = weatherNotification.WindDirection,

            };
    }

    //make non-blocking request to forward location with WeatherData object (internal version)
    private async Task GenerateForwardRequest(WeatherData weather)
    {
        try
        {
            //using statements to dispose of objects when done being used, and not prior
            using (HttpClient client = new HttpClient())
            {
                StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(weather));
                using (HttpResponseMessage response = await client.PostAsync(_Forward, jsonContent))
                {
                    var statusCode = response.EnsureSuccessStatusCode();
                    if (statusCode.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Weather Forwarded:{weather}");
                    }

                    //potential here for options to resend, multiple attempts through config options, etc. 
                    else
                    {
                        Debug.WriteLine($"Weather failed to Forward:{weather.date}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


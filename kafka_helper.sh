#!/bin/bash
# If you get the following error:
#   Error: /bin/bash^M: bad interpreter: No such file or directory
#   Solution: sed -i -e 's/\r$//' filename.sh

# Helpers
GREEN='\033[92m'
RED='\033[91m'
NC='\033[0m'
CYAN='\033[96m'
YELLOW='\033[93m'
TICK="[${GREEN}✓${NC}]"
CROSS="[${RED}✗${NC}]"
INFO="[${CYAN}i${NC}]"
ALERT="[${YELLOW}⚠${NC}]"

listtopics() {
    docker run --tty --network coding_exercise_default confluentinc/cp-kafkacat kafkacat -b kafka:9092 -L
}

readdata() {
    docker run --tty \
    --network coding_exercise_default \
    confluentinc/cp-kafkacat \
    kafkacat -b kafka:9092 -C \
    -t WeatherData
}

pushdata() {
    docker run --network coding_exercise_default \
    --volume $(pwd)/weatherdata.json:/data/weatherdata.json \
    confluentinc/cp-kafkacat \
    kafkacat -b kafka:9092 \
    -t WeatherData \
    -P -l /data/weatherdata.json
}

help() {
    echo -e "Usage: kafka_helper [options]

Example: kafka_helper topics

Options:
    ${CYAN}topics${NC}      List all Kafka topics
    ${CYAN}pushdata${NC}    Push data from weatherdata.json onto Kafka topic
    ${CYAN}readdata${NC}    Read data from Kafka topic
    "
    exit 0
}

case "${1}" in
    "topics") listtopics ;;
    "pushdata") pushdata ;;
    "readdata") readdata ;;
    *) help ;;
esac

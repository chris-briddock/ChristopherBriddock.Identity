#!/bin/bash

# Function to display usage information
usage() {
    echo "Usage: $0 [OPTIONS]"
    echo "Options:"
    echo "  -h, --help                     Display this help message"
    echo "  --db-host HOST                 Database host"
    echo "  --db-port PORT                 Database port"
    echo "  --db-username USERNAME         Database username"
    echo "  --db-password PASSWORD         Database password"
    echo "  --db-name NAME                 Database name"
    echo "  --redis-connection-string STR  Redis connection string"
    echo "  --redis-instance-name NAME     Redis instance name"
    echo "  --jwt-issuer ISSUER            JWT issuer"
    echo "  --jwt-audience AUDIENCE        JWT audience"
    echo "  --jwt-secret SECRET            JWT secret"
    echo "  --jwt-expires EXPIRES          JWT expiration"
    echo "  --app-insight-key KEY          Application Insight instrumentation key"
    echo "  --seq-endpoint ADDRESS         Seq endpoint address"
    echo "  --seq-port PORT                Seq port"
    echo "  --rabbit-host HOST              RabbitMQ host"
    echo "  --rabbit-username USERNAME     RabbitMQ username"
    echo "  --rabbit-password PASSWORD     RabbitMQ password"
    echo "  --service-bus-conn STRING      Azure Service Bus connection string"
    echo "  --seq-api-key KEY              Seq API key"
    echo "  --email-address ADDRESS        Email address"
    echo "  --email-password PASSWORD      Email password"
    echo "  --enable-feature FEATURE       Enable feature flag (true/false)"
    echo "  --disable-feature FEATURE      Disable feature flag (true/false)"
    exit 1
}

# Parse command-line arguments
if [ "$#" -eq 0 ]; then
    usage
fi

while [[ $# -gt 0 ]]; do
    key="$1"
    case $key in
        -h|--help)
            usage
            ;;
        --db-host)
            dbHost="$2"
            shift
            shift
            ;;
        --db-port)
            dbPort="$2"
            shift
            shift
            ;;
        --db-username)
            dbUsername="$2"
            shift
            shift
            ;;
        --db-password)
            dbPassword="$2"
            shift
            shift
            ;;
        --db-name)
            dbName="$2"
            shift
            shift
            ;;
        --redis-connection-string)
            redisConnectionString="$2"
            shift
            shift
            ;;
        --redis-instance-name)
            redisInstanceName="$2"
            shift
            shift
            ;;
        --jwt-issuer)
            jwtIssuer="$2"
            shift
            shift
            ;;
        --jwt-audience)
            jwtAudience="$2"
            shift
            shift
            ;;
        --jwt-secret)
            jwtSecret="$2"
            shift
            shift
            ;;
        --jwt-expires)
            jwtExpires="$2"
            shift
            shift
            ;;
        --app-insight-key)
            appInsightInstrumentationKey="$2"
            shift
            shift
            ;;
        --seq-endpoint)
            seqEndpointAddress="$2"
            shift
            shift
            ;;
        --seq-port)
            seqPort="$2"
            shift
            shift
            ;;
        --rabbit-host)
            hostname="$2"
            shift
            shift
            ;;
        --rabbit-username)
            rabbitUsername="$2"
            shift
            shift
            ;;
        --rabbit-password)
            rabbitPassword="$2"
            shift
            shift
            ;;
        --service-bus-conn)
            connectionString="$2"
            shift
            shift
            ;;
        --seq-api-key)
            seqApiKey="$2"
            shift
            shift
            ;;
        --email-address)
            emailAddress="$2"
            shift
            shift
            ;;
        --email-password)
            emailPassword="$2"
            shift
            shift
            ;;
        --enable-feature)
            feature="$2"
            featureValue="true"
            shift
            shift
            ;;
        --disable-feature)
            feature="$2"
            featureValue="false"
            shift
            shift
            ;;
        *)
            echo "Invalid option: $1"
            usage
            ;;
    esac
done

# Replace placeholder values in the appsettings.json file
if [ ! -z "$dbHost" ]; then
    sed -i "s/{dbHost}/$(echo "$dbHost" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$dbPort" ]; then
    sed -i "s/{dbPort}/$(echo "$dbPort" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$dbUsername" ]; then
    sed -i "s/{dbUsername}/$(echo "$dbUsername" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$dbPassword" ]; then
    sed -i "s/{dbPassword}/$(echo "$dbPassword" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$dbName" ]; then
    sed -i "s/{dbName}/$(echo "$dbName" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$redisConnectionString" ]; then
    sed -i "s/{redisConnectionString}/$(echo "$redisConnectionString" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$redisInstanceName" ]; then
    sed -i "s/{redisInstanceName}/$(echo "$redisInstanceName" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$jwtIssuer" ]; then
    sed -i "s/{jwtIssuer}/$(echo "$jwtIssuer" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$jwtAudience" ]; then
    sed -i "s/{jwtAudience}/$(echo "$jwtAudience" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$jwtSecret" ]; then
    sed -i "s/{jwtSecret}/$(echo "$jwtSecret" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$jwtExpires" ]; then
    sed -i "s/{jwtExpires}/$(echo "$jwtExpires" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$appInsightInstrumentationKey" ]; then
    sed -i "s/{appInsightInstrumentationKey}/$(echo "$appInsightInstrumentationKey" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$seqEndpointAddress" ]; then
    sed -i "s/{seqEndpointAddress}/$(echo "$seqEndpointAddress" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$seqPort" ]; then
    sed -i "s/{seqPort}/$(echo "$seqPort" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$hostname" ]; then
    sed -i "s/{hostname}/$(echo "$hostname" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$rabbitUsername" ]; then
    sed -i "s/{rabbitUsername}/$(echo "$rabbitUsername" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$rabbitPassword" ]; then
    sed -i "s/{rabbitPassword}/$(echo "$rabbitPassword" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$connectionString" ]; then
    sed -i "s/{connectionString}/$(echo "$connectionString" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$seqApiKey" ]; then
    sed -i "s/{seqApiKey}/$(echo "$seqApiKey" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$feature" ]; then
    sed -i "s/\"$feature\": .*,$/\"$feature\": $featureValue,/g" appsettings.json
fi

if [ ! -z "$emailAddress" ]; then
    sed -i "s/{emailAddress}/$(echo "$emailAddress" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

if [ ! -z "$emailPassword" ]; then
    sed -i "s/{emailPassword}/$(echo "$emailPassword" | sed 's/[\/&]/\\&/g')/g" appsettings.json
fi

echo "Placeholder values replaced in appsettings.json file."
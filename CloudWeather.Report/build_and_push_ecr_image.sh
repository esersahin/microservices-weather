#!/bin/bash
set -e

aws ecr get-login-password --region us-east-1 --profile weather-ecr-agent | docker login --username AWS --password-stdin 931995447723.dkr.ecr.eu-central-1.amazonaws.com
docker build -f ./Dockerfile -t cloudweather-report:latest .
docker tag cloud-weather-report:latest 931995447723.dkr.ecr.eu-central-1.amazonaws.com/cloud-weather-report:latest
docker push 931995447723.dkr.ecr.eu-central-1.amazonaws.com/cloud-weather-report:latest

# # Build the docker image
# docker build -t cloudweather-temperature .

# # Tag the image
# docker tag cloudweather-temperature:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/cloudweather-temperature:latest

# # Push the image to ECR
# docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/cloudweather-temperature:latest

# # Path: CloudWeather.Temperature/ecs_task_definition.json
# {
#     "family": "cloudweather-temperature",
#     "taskRoleArn": "arn:aws:iam::123456789012:role/ecsTaskExecutionRole",
#     "executionRoleArn": "arn:aws:iam::123456789012:role/ecsTaskExecutionRole",
#     "networkMode": "awsvpc",
#     "containerDefinitions": [
#         {
#             "name": "cloudweather-temperature",
#             "image": "123456789012.dkr.ecr.us-east-1.amazonaws.com/cloudweather-temperature:latest",
#             "cpu": 256,
#             "memory": 512,
#             "portMappings": [
#                 {
#                     "containerPort": 80,
#                     "hostPort": 80,
#                     "protocol": "tcp"
#                 }
#             ],
#             "essential": true,
#             "environment": [
#                 {
#                     "name": "ASPNETCORE_ENVIRONMENT",
#                     "value": "Production"
#                 },
#                 {
#                     "name": "ASPNETCORE_URLS",
#                     "value": "http://+:80"
#                 }
#             ],
#             "logConfiguration": {
#                 "logDriver": "awslogs",
#                 "options": {
#                     "awslogs-group": "cloudweather-temperature",
#                     "awslogs-region": "us-east-1",
#                     "awslogs-stream-prefix": "cloudweather-temperature"
#                 }
#             }
#         }
#     ],
#     "requiresCompatibilities": [
#         "FARGATE"
#     ],
#     "cpu": "256",
#     "memory": "512"
# }

# # Path: CloudWeather.Temperature/ecs_service.json
# {
#     "cluster": "cloudweather",
#     "serviceName": "cloudweather-temperature",
#     "taskDefinition": "cloudweather-temperature",
#     "desiredCount": 1,
#     "launchType": "FARGATE",
#     "networkConfiguration": {
#         "awsvpcConfiguration": {
#             "subnets": [
#                 "subnet-12345678901234567",
#                 "subnet-12345678901234567"
#             ],
#             "securityGroups": [
#                 "sg-12345678901234567"
#             ],
#             "assignPublicIp": "ENABLED"
#         }
#     },
#     "loadBalancers": [
#         {
#             "targetGroupArn": "arn:aws:elasticloadbalancing:us-east-1:123456789012:targetgroup/cloudweather-temperature/123456789012345678",
#             "containerName": "cloudweather-temperature",
#             "containerPort": 80
#         }
#     ],
#     "healthCheckGracePeriodSeconds": 60,
#     "schedulingStrategy": "REPLICA"
# }

# # Path: CloudWeather.Temperature/ecs_service.sh
# #!/bin/bash
# set -e

# # Create the task definition
# aws ecs register-task-definition --cli-input-json file://ecs_task_definition.json

# # Create the service
# aws ecs create-service --cli-input-json file://ecs_service.json

# # Path: CloudWeather.Temperature/ecs_service_update.sh
# #!/bin/bash
# set -e

# # Update the service
# aws ecs update-service --cluster cloudweather --service cloudweather-temperature --task-definition cloudweather-temperature

# # Path: CloudWeather.Temperature/ecs_service_delete.sh
# #!/bin/bash
# set -e

# # Delete the service
# aws ecs delete-service --cluster cloudweather --service cloudweather-temperature

# # Delete the task definition
# aws ecs deregister-task-definition --task-definition cloudweather-temperature

# # Path: CloudWeather.Temperature/ecs_service_logs.sh
# #!/bin/bash
# set -e

# # Get the log group name
# LOG_GROUP_NAME=$(aws ecs describe-services --cluster cloudweather --services cloudweather-temperature | jq -r '.services[0].taskDefinition' | xargs aws ecs describe-task-definition | jq -r '.taskDefinition.containerDefinitions[0].logConfiguration.options."awslogs-group"')

# # Get the log stream name
# LOG_STREAM_NAME=$(aws logs describe-log-streams --log-group-name $LOG_GROUP_NAME | jq -r '.logStreams[0].logStreamName')

# # Get the log events
# aws logs get-log-events --log-group-name $LOG_GROUP_NAME --log-stream-name $LOG_STREAM_NAME

# # Path: CloudWeather.Temperature/ecs_service_logs.sh
# #!/bin/bash
# set -e

# # Get the log group name
# LOG_GROUP_NAME=$(aws ecs describe-services --cluster cloudweather --services cloudweather-temperature | jq -r '.services[0].taskDefinition' | xargs aws ecs describe-task-definition | jq -r '.taskDefinition.containerDefinitions[0].logConfiguration.options."awslogs-group"')

# # Get the log stream name
# LOG_STREAM_NAME=$(aws logs describe-log-streams --log-group-name $LOG_GROUP_NAME | jq -r '.logStreams[0].logStreamName')

# # Get the log events
# aws logs get-log-events --log-group-name $LOG_GROUP_NAME --log-stream-name $LOG_STREAM_NAME

# # Path: CloudWeather.Temperature/ecs_service_logs.sh
# #!/bin/bash
# set -e

# # Get the log group name
# LOG_GROUP_NAME=$(aws ecs describe-services --cluster cloudweather --services cloudweather-temperature | jq -r '.services[0].taskDefinition' | xargs aws ecs describe-task-definition | jq -r '.taskDefinition.containerDefinitions[0].logConfiguration.options."awslogs-group"')

# # Get the log stream name
# LOG_STREAM_NAME=$(aws logs describe-log-streams --log-group-name $LOG_GROUP_NAME | jq -r '.logStreams[0].logStreamName')

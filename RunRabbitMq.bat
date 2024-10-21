podman run -d --network myapp_net --hostname rabbitmq --name rabbitmq -p 15672:15672 -p 5672:5672 --rm -e RABBITMQ_DEFAULT_USER='user' -e RABBITMQ_DEFAULT_PASS='password' rabbitmq:3-management 

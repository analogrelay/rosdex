FROM docker.elastic.co/elasticsearch/elasticsearch:5.5.2

# Bring in our config
COPY elasticsearch.yml /usr/share/elasticsearch/config

# Switch to root and chown the config over to elasticsearch
USER root
RUN chown elasticsearch:elasticsearch config/elasticsearch.yml

# Switch back to elasticsearch user.
USER elasticsearch
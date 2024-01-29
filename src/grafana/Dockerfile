FROM grafana/grafana:latest

COPY ./grafana.ini /etc/grafana/grafana.ini

EXPOSE 3000

CMD ["grafana-server", "--config=/etc/grafana/grafana.ini"]

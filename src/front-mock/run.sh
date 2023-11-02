rm -rf /root/devops/ynhzc-web/dist/*
mv -f /root/devops/ynhzc-web/deploy/* /root/devops/ynhzc-web/dist
rm -rf /root/devops/ynhzc-web/deploy
docker compose up --build > /dev/null 2>&1 &
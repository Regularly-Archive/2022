#!/bin/bash
time=`date`
echo $time > /tmp/clear_cached.txt

echo "total/used/free/shared/buffers/cached" >> /tmp/clear_cached.txt
echo "Cleart Before" >> /tmp/clear_cached.txt
free -m|grep Mem: >> /tmp/clear_cached.txt
sync;
echo 2 > /proc/sys/vm/drop_caches;
echo 0 > /proc/sys/vm/drop_caches;
echo "Clear After" >> /tmp/clear_cached.txt 
free -m|grep Mem:  >> /tmp/clear_cached.txt
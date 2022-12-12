# add-watermark nextcloud/files/KAFKA.pdf nextcloud
#/bin/sh 
exec 2>/tmp/trace.log 
set -x
dataDir="/var/www/html/data"
echo "add watermark for pdf file: $1 ..."
stamp=`date '+%d/%m/%Y  %H:%M:%S'`
/etc/pdfstamp.sh "$2 $stamp" > /tmp/watermark.pdf
pdftk /var/www/html/data/$1 stamp /tmp/watermark.pdf output $dataDir/nextcloud/files/output.pdf
rm -f /tmp/watermark.pdf
rm -f /var/www/html/data/$1
mv $dataDir/nextcloud/files/output.pdf /var/www/html/data/$1
/var/www/html/occ files:scan $2



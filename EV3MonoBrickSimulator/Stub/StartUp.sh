/*This is just a copy of the starup script file from the firmware image - used to satisfy firmware update stubs*/
#! /bin/sh
#echo "Start WiFi"
#/lejos/bin/startwlan
. /etc/default/lejos
${LEJOS_HOME}/bin/startnetwork
#echo "Mount dev system"
#busybox mount -t nfs -o nolock legovm:/export/Lego /legovm
# show lejos splash, while modules load
echo "Load modules"
${LEJOS_HOME}/bin/init
echo "Load mono startup"
if [ ! -f "/usr/local/bin/StartupApp.exe.so" ]; then
  echo "AOT compiling startup app"
  /usr/local/bin/mono --aot=full /usr/local/bin/StartupApp.exe >> /home/root/startuplog 2>&1
  /usr/local/bin/mono --aot=full /usr/local/bin/MonoBrickFirmware.dll >> /home/root/startuplog 2>&1
  /usr/local/bin/mono --aot=full /usr/local/bin/StartupApp.XmlSerializers.dll >> /home/root/startuplog 2>&1
fi
/usr/local/bin/mono --full-aot /usr/local/bin/StartupApp.exe >> /home/root/startuplog 2>&1  &
echo "Complete"




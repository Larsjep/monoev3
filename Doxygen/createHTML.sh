#!/bin/bash 
doxyGenExecute=/Applications/Doxygen.app/Contents/Resources/doxygen
currentDir=${PWD}
outputDir=$currentDir 
#*******
#Set these parameters to match your settings
logoFileName=monobrickLogo200.png
monoBrickDirName=MonoBrickFirmware
configFileName=doxygenConfig
#End of set
#********
if [ ! -f "$doxyGenExecute" ]; then
    doxyGenExecute="doxygen"
fi

if [ -z "$1" ]
  then
   echo "No output path set using current directory" $outputDir
  else
     outputDir=$1
     echo "Output path is set to "  $outputDir
fi


 
cd $currentDir
doxyGenLogo=$currentDir/$logoFileName
doxyGenConfig=$currentDir/$configFileName
cd ..
monoBrickDir=${PWD}/$monoBrickDirName
cd $currendDir		
cp $doxyGenLogo $monoBrickDir
cd $monoBrickDir
$doxyGenExecute $currentDir/$configFileName
rm $monoBrickDir/$logoFileName
mv html $outputDir

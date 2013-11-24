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

if [ -z "$1" ]
  then
   echo "Please specify a path to doxygen"
   exit 1
  else
     doxyGenExecute=$1
     echo "Doxygen path specified to "  $doxyGenExecute  
fi

if [ -z "$2" ]
  then
   echo "No output path set using current directory" $outputDir
  else
     outputDir=$2
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

RoboDog
=======

A Mindstorms Project based on monoev3
This project is build upon the monoev3, and will only work if one has downloaded the EV3-firmware from monobrick.dk.
The software mimics that of Legos own software for the Puppy (found in the educational set), but can be extended much further thanks to the freedom in C#.
The reason I build this software is that I use it as a teaching tool in the world of object oriented programming. Everything becomes very concrete when code is added to a "real" dog.
My hope is to add several teaching tools, so that other teachers can benefit from the same software.
RoboDog includes all eye-images, but sound is not yet supported. I am also currently working on using interfaces rather than classes so that things will be less "hardcoded", which is why you will find an AbstractDog.cs and an AbstractDog2.cs. The ExternalMotor class is an attemt to wrap an existing class in order for it to use interfaces.
Besides, the dogs movements may need some adjustment, however, simply override the methods from the abstract dog, and you will be able to implemnent your only movements. 

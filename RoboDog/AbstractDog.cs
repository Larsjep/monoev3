using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
//using MonoBrick.EV3;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sound;



namespace RoboDog
{
    [Serializable]
    public abstract class AbstractDog : IDog
    {
       
        private IMemory memory;
        public IDictionary<string, Action> combos;

        private DateTime dateOfBirth;
        private int age;
        private int bladderLevel;
        private int happiness;
        private int fullness;
        private string name;
        private DogSound sound;
        private DogEyes eyes;

        public AbstractDog(IMemory memory)
        {
            combos = new Dictionary<string, Action>();
            this.memory = memory;
            this.dateOfBirth = DateTime.Now;
            this.age = 0;
            this.fullness = 10;
            this.happiness = 12;
            this.bladderLevel = 0;
            this.name = "RoboDog";
            this.eyes = DogEyes.Neutral;
          
             
            StartLife();
        }
        public AbstractDog(IMemory memory, int fullness, int happiness, int bladderlevel, string name, DogEyes eyes)
        {
            combos = new Dictionary<string, Action>();

            this.memory = memory;
            this.dateOfBirth = DateTime.Now;
            this.age = 0;
            this.fullness = fullness;
            this.happiness = happiness;
            this.bladderLevel = bladderlevel;
            this.name = name;
            this.eyes = eyes;
           

            StartLife();
        }

        private void StartLife()
        {
            //Call the aging (mood) every minute the Dog is alive
            Timer t = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(this.Aging);
            t.Start();
        }


        public virtual DateTime DateOfBirth
        {
            get { return dateOfBirth; }
        }

        public virtual int Age
        {
            get { return age; }
        }

        public virtual int BladderLevel
        {
            get { return bladderLevel; }
            set { bladderLevel = value; }
        }

        public virtual int Happiness
        {
            get { return happiness; }
            set { happiness = value; }
        }

        public virtual int Fullness
        {
            get { return fullness; }
            set { fullness = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual DogEyes Eyes
        {
            get { return eyes; }
        }

        public virtual DogSound Sound
        {
            get { return sound; }
        }

        
        public virtual void Drink()
        {
            MakeSound(DogSound.Crunching);//I don't think there is a drinking sound
            bladderLevel += 2;
            LcdConsole.WriteLine("Drinking");

            //make dog pee 
            if (bladderLevel > 10)
                Pee();
        }

        public virtual void Eat()
        {
            //Make crunching sound
            MakeSound(DogSound.Crunching);
            fullness += 4;
            LcdConsole.WriteLine("Eating");
        }

        
        public virtual void Pee()
        {
            //Lift leg 
            
            Motor motor = new Motor(MotorPort.OutA);
            motor.ResetTacho();
            motor.SetSpeed(20);
            System.Threading.Thread.Sleep(2500);
            motor.Brake();
            //System.Threading.Thread.Sleep(2500);

            //Make peeing sound
            MakeSound(DogSound.Pee);
            LcdConsole.WriteLine("Peeing");
              
            //Reset bladder
            BladderLevel = 0;
        }
        public virtual void HeadUp()
        {
            //TODO: Make the right moves
            Motor m = new Motor(MotorPort.OutC);
            m.SpeedProfileTime(-30, 70, 700, 70, true);
          
        }
        public virtual void HeadDown()
        {
            //TODO: Make the right moves
            Motor m = new Motor(MotorPort.OutC);
            m.SpeedProfileTime(30, 70, 700, 70, true);
        }

        public virtual void Pet()
        {
            happiness += 3;
            MakeSound(DogSound.Bark2);
            ControlEyes(DogEyes.Love);
        }

        public virtual void Search()
        {
            MakeSound(DogSound.Sniff);
            LcdConsole.WriteLine("Searching");
        }

        public virtual void Sit()
        {
            //TODO: Make the right moves
            MotorSync m = new MotorSync(MotorPort.OutA, MotorPort.OutD);
            m.TimeSync(99, 0, 700, true);
            
        }

        public virtual void Stay()
        {
            Motor m = new Motor(MotorPort.OutC);
            m.SpeedProfileTime(-30, 70, 4000, 70, true);
            System.Threading.Thread.Sleep(4000  );
            //TODO: Make the right moves
            MotorSync ms = new MotorSync(MotorPort.OutA, MotorPort.OutD);
            ms.TimeSync(-99, 0, 1000, true);
        }

        public virtual void ControlEyes(DogEyes eyes)
        {
            //use enum for picking the right image file
            this.eyes = eyes;   
            LcdConsole.WriteLine(eyes.ToString());
            Bitmap bmpEyes = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), eyes.ToString() + ".bitmap");
            Lcd.Instance.Clear();
            Lcd.Instance.DrawBitmap(bmpEyes, new Point((int)(Lcd.Width - bmpEyes.Width) / 2, 10));
            Lcd.Instance.Update();	
           
        }

        public virtual void MakeSound(DogSound sound)
        {
            //TODO: Play sound from file
            this.sound = sound;
            //use enum for picking the right sound file
            string soundFileName = "/home/root/apps/SoundTest.wav";
            //Maybe this directory:  /home/root/lms2012/prjs/Hvalp/Dog sniff.rsf

            var speaker = new Speaker(50);
            try
            {
               // speaker.PlaySoundFile(soundFileName);
                speaker.Beep();
            }
            catch (Exception e)
            {
                LcdConsole.WriteLine("Failed to play " + soundFileName);
                LcdConsole.WriteLine("Exception" + e.Message);
                LcdConsole.WriteLine("Stack trace " + e.StackTrace);
            }

        }

        private void Aging(object sender, ElapsedEventArgs e)
        {
            TimeSpan ts = DateTime.Now - dateOfBirth;
            age = ts.Minutes;

            fullness--;
            happiness--;
            if (fullness < 3)
            {
                //if hungry, accelerate unhappiness
                happiness--;
            }
            Mood();
        }

        private void Mood()
        {
            if (happiness > 10)
            {
                //Bark1
                MakeSound(DogSound.Bark1);

                //Eyes awake
                ControlEyes(DogEyes.Neutral);

            }
            else if (happiness > 9)
            {
                //Whine
                MakeSound(DogSound.Bark2);

                //Sad Eyes 
                ControlEyes(DogEyes.Up);
            }
            else if (happiness > 8)
            {
                //Whine
                MakeSound(DogSound.Bark2);

                //Sad Eyes 
                ControlEyes(DogEyes.Down);
            }
            else if (happiness > 7)
            {
                //Whine
                MakeSound(DogSound.Bark2);

                //Sad Eyes 
                ControlEyes(DogEyes.Awake);
            }
            else if (happiness > 5)
            {
                //Whine
                MakeSound(DogSound.Sniff);

                //Sad Eyes 
                ControlEyes(DogEyes.Disappointed);
            }
            else if (happiness > 2)
            {
                //Whine
                MakeSound(DogSound.Whine);

                //Sad Eyes 
                ControlEyes(DogEyes.Hurt);
            }
            else if (happiness > 0)
            {
                //Growl
                MakeSound(DogSound.Growl);

                //Angry Eyes
                ControlEyes(DogEyes.Angry);
            }
            else
            {
                //Sit position
                Sit();
                //No sound - the dog is dead

                //Closed eyes
                ControlEyes(DogEyes.KnockedOut);
            }


        }

        public override string ToString()
        {
            return Name;
        }

        public void DoTrick(string name)
        {
            if (combos.ContainsKey(name))
            {
                combos[name].DynamicInvoke(null);
            }
        }

        public virtual IMemory Memory
        {
            get { return memory; }
        }
       
        public virtual void AddTrickToMemory(string name)
        {
            IDictionary<string, Action> newDict = new Dictionary<string, Action>();
            foreach (KeyValuePair<string, Action> entry in combos)
            {
                if (entry.Key == name)
                {
                    newDict.Add(entry.Key, entry.Value);
                   // memory.AddTrickToMemory(name, (entry.Value.Method).ToString());
                    memory.AddTrickToMemory(newDict);
                }
            }
           
        }

        public virtual void AddAllTricksToMemory()
        {
            memory.AddAllTricksToMemory(combos);
        }

        public virtual IDictionary<string, Action> GetTricksFromMemory()
        {
            return memory.GetTricksFromMemory();
        }

    }
}

using System;
using System.Collections.Generic;
namespace RoboDog
{
    public interface IDog
    {
        
        IMemory Memory { get; }
        int Age { get; }
        int BladderLevel { get; set; }
        string Name { get; set; }
        int Fullness { get; set; }
        int Happiness { get; set; }
        DateTime DateOfBirth { get; }
        DogEyes Eyes { get; }
        DogSound Sound { get; }        
        void ControlEyes(DogEyes eyes);
        void MakeSound(DogSound sound);
        void Pee();
        void Pet();
        void Search();
        void Sit();
        void Drink();
        void Eat();
        void Stay();
        string ToString();
        void DoTrick(string name);
        void AddTrickToMemory(string name);
        void AddAllTricksToMemory();
        IDictionary<string, Action> GetTricksFromMemory();
    }
}

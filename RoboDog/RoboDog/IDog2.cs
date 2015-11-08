using System;
using System.Collections.Generic;
namespace RoboDog
{
    interface IDog2
    {
        int Age { get; }
        void Aging();
        int BladderLevel { get; set; }
        void ControlEyes(DogEyes eyes);
        DateTime DateOfBirth { get; }
        void Drink();
        DogEyes Eyes { get; }
        int Fullness { get; set; }
        int Happiness { get; set; }
        void MakeSound(DogSound sound);
        IMemory Memory { get; }
        void Mood();
        string Name { get; set; }
        void Pee();
        void Pet();
        void Search();
        void Sit();
        DogSound Sound { get; }
        void Stay();
        string ToString();
        void DoTrick(string name);
        void AddTrickToMemory(string name);
        void AddAllTricksToMemory();
        IDictionary<string, Action> GetTricksFromMemory();
    }
}

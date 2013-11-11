using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConquerorClient
{
    //All enums are designed so that they may be cast to characters and passed immediately to API

    public enum GameState
    {
        Waiting  = 'W',
        Active   = 'A',
        Finished = 'F'
    }

    public enum GameType
    {
        Standard   = 'S',
        Terminator = 'C',
        Assassin   = 'A',
        Doubles    = 'D',
        Triples    = 'T',
        Quadruples = 'Q'
    }

    public enum InitialTroops
    {
        Automatic = 'E',
        Manual    = 'M'
    }

    public enum PlayOrder
    {
        Sequential = 'S',
        Freestyle  = 'F'
    }

    public enum Spoils
    {
        None       = '1',
        Escalating = '2',
        FlatRate   = '3',
        Nuclear    = '4'
    }

    public enum Reinforcements
    {
        Chained     = 'C',
        Adjacent    = 'O',
        Unlimited   = 'M'
    }

    public enum PrivateGame
    {
        No          = 'N',
        Yes         = 'Y',
        Tournament  = 'T'
    }
}

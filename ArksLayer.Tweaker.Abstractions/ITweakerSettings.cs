using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Using this interface, various settings required by the Tweaker can be read strongly.
    /// </summary>
    public interface ITweakerSettings
    {
        /// <summary>
        /// Sets or gets the value of the directory path to the English Large Patch version.
        /// </summary>
        string EnglishLargePatchVersion { set; get; }

        /// <summary>
        /// Sets or gets the value of the directory path to the English Patch version.
        /// </summary>
        string EnglishPatchVersion { set; get; }

        /// <summary>
        /// Sets or gets the value of the directory path to pso2_bin.
        /// Expected result example: D:\PSO2\pso2_bin
        /// </summary>
        string GameDirectory { set; get; }

        /// <summary>
        /// Sets or gets the value of the game client version.
        /// </summary>
        string GameVersion { set; get; }

        /// <summary>
        /// Sets or gets the value of the directory path to the Story Patch version.
        /// </summary>
        string StoryPatchVersion { set; get; }
    }
}

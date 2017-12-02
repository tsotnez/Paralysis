using System.Collections.Generic;
/// <summary>
/// Holds information about each Hero and his skills in dictionaries named after the hero it contains information about.
/// </summary>
public static class ChampionDatabase {

    public enum Keys
    {
        Skill1Name, Skill2Name, Skill3Name, Skill4Name, Skill1Text, Skill2Text, Skill3Text, Skill4Text, Lore, Name
    }

    //Set Values for each Dictionary (hardcoded)

    #region Assassin
    public static Dictionary<Keys, string> Assassin = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, "Paralysing Poison"},
        {Keys.Skill1Text, "Assassins blades are infused with a strong poison, stunning the enemy he hits."},

        {Keys.Skill2Name, "Vanish"},
        {Keys.Skill2Text, "A relic stealth generator makes Assassin become invsible. Focusing on his next targets weak spot, " +
            "Assassins next basic attack deals additional damage."},

        {Keys.Skill3Name, "Shadowstep"},
        {Keys.Skill3Text, "A quick teleport to the nearest target on same height allows Assassin to surprise his victim and stun it, " +
            "also dealing a small amount of damage."},

        {Keys.Skill4Name, "Longfang"},
        {Keys.Skill4Text, "Assassin shoots his pistol dealing massive damage and knocking back the poor victim."},

        {Keys.Name, "Assassin, the Shadowwalker"},
        {Keys.Lore, "This unknown warrior wandered trough the wastelands for a long time after the world faced its doom. He learned to " +
            "survive on his own and salvaged strong tech on his journeys which allow him to perform a variety of skills."}
    };
    #endregion



    #region Archer
    public static Dictionary<Keys, string> Archer = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, ""},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, ""},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, ""},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, ""},
        {Keys.Skill4Text, ""},

        {Keys.Name, ""},
        {Keys.Lore, ""}
    };
    #endregion

    #region Knight
    public static Dictionary<Keys, string> Knight = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, ""},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, ""},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, ""},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, ""},
        {Keys.Skill4Text, ""},

        {Keys.Name, ""},
        {Keys.Lore, ""}
    };
    #endregion

    #region Infantry
    public static Dictionary<Keys, string> Infantry = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, ""},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, ""},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, ""},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, ""},
        {Keys.Skill4Text, ""},

        {Keys.Name, ""},
        {Keys.Lore, ""}
    };
    #endregion

    #region Alchemist
    public static Dictionary<Keys, string> Alchemist = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, ""},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, ""},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, ""},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, ""},
        {Keys.Skill4Text, ""},

        {Keys.Name, ""},
        {Keys.Lore, ""}
    };
    #endregion

}

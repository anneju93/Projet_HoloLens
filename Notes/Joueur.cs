public class Joueur{
    int Lvl, MaxPV, PV, AC, ProfMod, Speed, MoveLeft, HitDiceSize, HitDiceLeft;
    int[] AbilityScores, AbilityMods
    string Name, Race, Class, SubClass, FightingStyle
    string[] Proficiencies, Inventory, Feats

    public Joueur(int lvl, int hitDiceSize, int[] scores){
        Lvl = 1;
        AbilityScores = scores;
        AbilityMods = GetModifiers(scores);
    }
    private int[] GetModifiers(int[] scores){
        int[] mods;
        for(int x = 0; x <= 5; x++){
            mods[x] = scores[x];
        }
        return new int[];
    }
}
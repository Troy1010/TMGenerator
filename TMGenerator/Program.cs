using System.Globalization;
using System.Text;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Synthesis;

namespace TMGenerator
{
    public class Program
    {
        private static Lazy<Settings> _settings = null!;
        private static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<IOblivionMod, IOblivionModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out _settings)
                .SetTypicalOpen(GameRelease.Oblivion, "TMGenerator.esp")
                .Run(args);
        }

        public static string StringToHex(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return BitConverter.ToString(byteArray).Replace("-", "");
        }

        public static uint HexStringToUInt(string hex)
        {
            // Ensure the string is valid and trim if necessary (depends on input string length)
            if (hex.Length > 8) // 8 hex digits = 32 bits, which is the limit for uint
            {
                throw new ArgumentOutOfRangeException("Hex string is too long to fit into a uint.");
            }

            return uint.Parse(hex, NumberStyles.HexNumber);
        }

        public static string ToLogStr(List<string> items)
        {
            // Use String.Join to concatenate list items with ", " as the delimiter
            return string.Join(", ", items);
        }

        public static void RunPatch(IPatcherState<IOblivionMod, IOblivionModGetter> state)
        {
            Console.WriteLine("\n\nRunPatch`Open\n");
            var changeCount = 0;
            // FeatureReduceAggressionForFCOM
            Console.WriteLine("\nFeatureReduceAggressionForFCOM\n");
            if (Settings.FeatureReduceAggressionForFCOM)
            {
                foreach (var oldCreature in state.LoadOrder.PriorityOrder.Creature().WinningContextOverrides())
                {
                    try
                    {
                        Console.WriteLine($"oldCreature.FormKey.ModKey.Name:{oldCreature.Record.FormKey.ModKey.Name}");
                        if (
                            !new List<string>()
                                {
                                    "Mart's Monster Mod",
                                    "Oscuro's_Oblivion_Overhaul",
                                    "Oblivion WarCry EV",
                                }
                                .Contains(oldCreature.Record.FormKey.ModKey.Name))
                        {
                            Console.WriteLine($"Skipping because record is not in FCOM.");
                            continue;
                        }

                        if (oldCreature.Record.AIData == null)
                        {
                            Console.WriteLine($"Skipping because AIData is null.");
                            continue;
                        }
                        var newCreature = oldCreature.Record.DeepCopy();
                        newCreature.AIData!.Aggression = 10; //TODO
                        state.PatchMod.Creatures.Set(newCreature);
                        changeCount++;
                    }
                    catch (Exception ex)
                    {
                        throw RecordException.Enrich(ex, 
                            oldCreature.Record.FormKey,
                            oldCreature.Record.GetType(),
                            oldCreature.Record.EditorID, 
                            oldCreature.ModKey);
                    }
                }
            }

            // FeatureTMFriendlierFactions
            Console.WriteLine("\nFeatureTMFriendlierFactions\n");
            if (Settings.FeatureTMFriendlierFactions)
            {
                foreach (var oldFaction in state.LoadOrder.PriorityOrder.WinningOverrides<IFactionGetter>())
                {
                    try
                    {
                        var allFCOMFriendlierFactions =
                            new List<Tuple<string, string>>()
                            {
                                Tuple.Create("00000013", "CreatureFaction"),
                                Tuple.Create("0000C0F2", "NecromancerFaction"),
                                Tuple.Create("000177E3", "BanditFaction"),
                                Tuple.Create("0001F1A6", "AdventurerFaction"),
                                Tuple.Create("00033F53", "VampireFaction"),
                                Tuple.Create("00035FD1", "MarauderFaction"),
                                Tuple.Create("0003E9A6", "DremoraFaction"),
                                Tuple.Create("0004B90B", "ConjurerFaction"),
                                Tuple.Create("0004B90C", "NecromancerDungeon"),
                                Tuple.Create("00053FED", "DaedraFaction"),
                                Tuple.Create("0005D556", "Prey"),
                                Tuple.Create("0005D557", "LichFaction"),
                                Tuple.Create("000700BF", "AnvilSirens"),
                                Tuple.Create("0009DB1F", "UndeadFaction"),
                                Tuple.Create("0500CD9C", "ZOOOBears"),
                                Tuple.Create("0500CD9D", "ZOOOBoars"),
                                Tuple.Create("0500CD9F", "ZOOOCougars"),
                                Tuple.Create("0500CDA0", "ZOOOMinotaurs"),
                                Tuple.Create("0500CDA1", "ZOOOOgres"),
                                Tuple.Create("0500CDA2", "ZOOOSpriggan"),
                                Tuple.Create("0500CDA3", "ZOOOSlaughterfish"),
                                Tuple.Create("0500CDA4", "ZOOORats"),
                                Tuple.Create("0500CDA5", "ZOOOWillowisps"),
                                Tuple.Create("0500CDA7", "ZOOOMudcrabs"),
                                Tuple.Create("0500CDA8", "ZOOOWolves"),
                                Tuple.Create("0500CDA9", "ZOOOWargs"),
                                Tuple.Create("0500CDAB", "ZOOOImps"),
                                Tuple.Create("0500CDAC", "ZOOOTrolls"),
                                Tuple.Create("0500D2BD", "ZOOOLandDreughs"),
                                Tuple.Create("050210EE", "ZOOOGoblinFaction"),
                                Tuple.Create("050417E6", "ZOOOGargoyles"),
                                Tuple.Create("07004CF4", "zMMMSpecial"),
                                Tuple.Create("07004D7E", "zzMMMRanger"),
                                Tuple.Create("0700DC77", "zMMMBear"),
                                Tuple.Create("0700DC78", "zMMMBoar"),
                                Tuple.Create("0700DC79", "zMMMGoblin"),
                                Tuple.Create("0700DC7A", "zMMMImp"),
                                Tuple.Create("0700DC7C", "zMMMBigCat"),
                                Tuple.Create("0700DC7D", "zMMMOgre"),
                                Tuple.Create("0700DC7E", "zMMMinotaur"),
                                Tuple.Create("0700DC7F", "zMMMTroll"),
                                Tuple.Create("0700DC80", "zMMMGiant"),
                                Tuple.Create("0700DC81", "zMMMGolem"),
                                Tuple.Create("0700DC82", "zMMMCrabs"),
                                Tuple.Create("0700DC83", "zMMMFish"),
                                Tuple.Create("0700DC84", "zMMMWolves"),
                                Tuple.Create("0700E66F", "zMMMLandDreugh"),
                                Tuple.Create("0700E670", "zMMMSpriggan"),
                                Tuple.Create("0700FA29", "zMMMRat"),
                                Tuple.Create("0701040C", "MMPRoamingTradersFaction"),
                                Tuple.Create("07025A94", "WildernessFaction"),
                            };
                        var factionsIWantToBeNicerToEachother =
                            new List<Tuple<string, string>>()
                            {
                                Tuple.Create("00000013", "CreatureFaction"),
                                Tuple.Create("0000C0F2", "NecromancerFaction"),
                                Tuple.Create("000177E3", "BanditFaction"),
                                // Tuple.Create("0001F1A6", "AdventurerFaction"),
                                Tuple.Create("00033F53", "VampireFaction"),
                                Tuple.Create("00035FD1", "MarauderFaction"),
                                Tuple.Create("0003E9A6", "DremoraFaction"),
                                Tuple.Create("0004B90B", "ConjurerFaction"),
                                Tuple.Create("0004B90C", "NecromancerDungeon"),
                                Tuple.Create("00053FED", "DaedraFaction"),
                                // Tuple.Create("0005D556", "Prey"),
                                Tuple.Create("0005D557", "LichFaction"),
                                Tuple.Create("000700BF", "AnvilSirens"),
                                Tuple.Create("0009DB1F", "UndeadFaction"),
                                Tuple.Create("0500CD9C", "ZOOOBears"),
                                Tuple.Create("0500CD9D", "ZOOOBoars"),
                                Tuple.Create("0500CD9F", "ZOOOCougars"),
                                Tuple.Create("0500CDA0", "ZOOOMinotaurs"),
                                Tuple.Create("0500CDA1", "ZOOOOgres"),
                                Tuple.Create("0500CDA2", "ZOOOSpriggan"),
                                Tuple.Create("0500CDA3", "ZOOOSlaughterfish"),
                                Tuple.Create("0500CDA4", "ZOOORats"),
                                Tuple.Create("0500CDA5", "ZOOOWillowisps"),
                                Tuple.Create("0500CDA7", "ZOOOMudcrabs"),
                                Tuple.Create("0500CDA8", "ZOOOWolves"),
                                Tuple.Create("0500CDA9", "ZOOOWargs"),
                                Tuple.Create("0500CDAB", "ZOOOImps"),
                                Tuple.Create("0500CDAC", "ZOOOTrolls"),
                                Tuple.Create("0500D2BD", "ZOOOLandDreughs"),
                                Tuple.Create("050210EE", "ZOOOGoblinFaction"),
                                Tuple.Create("050417E6", "ZOOOGargoyles"),
                                // Tuple.Create("07004CF4", "zMMMSpecial"),
                                // Tuple.Create("07004D7E", "zzMMMRanger"),
                                Tuple.Create("0700DC77", "zMMMBear"),
                                Tuple.Create("0700DC78", "zMMMBoar"),
                                Tuple.Create("0700DC79", "zMMMGoblin"),
                                Tuple.Create("0700DC7A", "zMMMImp"),
                                Tuple.Create("0700DC7C", "zMMMBigCat"),
                                Tuple.Create("0700DC7D", "zMMMOgre"),
                                Tuple.Create("0700DC7E", "zMMMinotaur"),
                                Tuple.Create("0700DC7F", "zMMMTroll"),
                                Tuple.Create("0700DC80", "zMMMGiant"),
                                Tuple.Create("0700DC81", "zMMMGolem"),
                                Tuple.Create("0700DC82", "zMMMCrabs"),
                                Tuple.Create("0700DC83", "zMMMFish"),
                                Tuple.Create("0700DC84", "zMMMWolves"),
                                Tuple.Create("0700E66F", "zMMMLandDreugh"),
                                Tuple.Create("0700E670", "zMMMSpriggan"),
                                Tuple.Create("0700FA29", "zMMMRat"),
                                // Tuple.Create("0701040C", "MMPRoamingTradersFaction"),
                                // Tuple.Create("07025A94", "WildernessFaction"),
                            };
                        if (!factionsIWantToBeNicerToEachother.Select(x => x.Item2).Contains(oldFaction.EditorID))
                        {
                            // Console.WriteLine($"Skipping oldFaction because EditorID:{oldFaction.EditorID} was not in factionsIWantToBeNicerToEachother.");
                            continue;
                        }

                        var newFaction = oldFaction.DeepCopy();

                        foreach (var relation in newFaction.Relations)
                        {
                            // TODO: This can give a false positive if records from 2 mods have the same last 6 "digits".
                            if (factionsIWantToBeNicerToEachother.Select(x => string.Concat(x.Item1.TakeLast(6))).Contains(string.Concat(relation.Faction.FormKey.ToString().Take(6))))
                            {
                                relation.Modifier = 100;
                            }
                        }

                        state.PatchMod.Factions.Set(newFaction);
                        changeCount++;
                        //Console.WriteLine($"Modified creature. FormKey.ModKey.Name:{newFaction.FormKey.ModKey.Name} EditorID:{newFaction.EditorID} Name:{newFaction.Name}");
                    }
                    catch (Exception ex)
                    {
                        throw RecordException.Enrich(ex, oldFaction);
                    }
                }
            }

            Console.WriteLine($"\n\nRunPatch`Close. changeCount:{changeCount}\n");
        }
    }
}
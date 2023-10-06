using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cobbs_Engine
{
    public static class Localization
    {
        public static event EventHandler<Language> LanguageSwitched;

        private static Language currentLanguage = Language.None;
        public static Language CurrentLanguage
        {
            get { return currentLanguage; }
            set { SwitchLanguage(value); }
        }
        private static Dictionary<Language, Dictionary<string, string>> LocalizationPairs = new();

        public static void Initialize()
        {
            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                // Skip "None" since it is not a valid language, and just a default state
                if (language == Language.None)
                    continue;

                LocalizationPairs.Add(language, IO.LoadLocalization($"{Enum.GetName(language)}.json"));
            }

            DetectLanguage();
        }

        public static void LoadContent()
        {

        }

        public static int LoadedLanguages => LocalizationPairs.Keys.Count > 0 ? LocalizationPairs.Values.Count(value => value != null) : 0;

        public static void AddLocalizationPair(string key, string value, Language language = Language.None)
        {
            if (language == Language.None && currentLanguage != Language.None)
            {
                language = CurrentLanguage;
            }
            else if (language == Language.None)
            {
                Diagnostics.LogError($"No input language for '{key}'");
                return;
            }

            if (LocalizationPairs[language].ContainsKey(key))
            {
                LocalizationPairs[language][key] = value;
            }
            else
            {
                LocalizationPairs[language].Add(key, value);
            }
        }

        public static string GetLocalizationPair(string key, Language language = Language.None, bool complain = true)
        {
            if (language == Language.None)
            {
                language = CurrentLanguage;
            }

            if (language != Language.None && LocalizationPairs[language] != null)
            {
                if (LocalizationPairs[language].ContainsKey(key))
                {
                    return LocalizationPairs[language][key];
                }
                else
                {
                    return key;
                    if (complain)
                        Diagnostics.LogWarning($"Localization pair '{key}' not found in {Enum.GetName(language)}");
                }
            }
            else
            {
                if (complain)
                    Diagnostics.LogError($"No language selected! Localization pair '{key}' not found");
                return key;
            }
        }

        public static bool LocalizationPairExists(string key, Language language = Language.None)
        {
            if (language == Language.None)
            {
                language = CurrentLanguage;
            }

            if (language != Language.None && LocalizationPairs.ContainsKey(language) && LocalizationPairs[language] != null)
            {
                return LocalizationPairs[language].ContainsKey(key);
            }
            else
            {
                return false;
            }
        }

        private static void SwitchLanguage(Language newLanguage)
        {
            if (LocalizationPairs[newLanguage] != null)
            {
                currentLanguage = newLanguage;
                Diagnostics.LogMessage($"Language Changed: {Enum.GetName(newLanguage)}");
                OnLanguageChanged(EventArgs.Empty, newLanguage);
                return;
            }
            Diagnostics.LogError($"Tried to change language to {Enum.GetName(newLanguage)}, but it did not exist");
        }

        public static void DetectLanguage()
        {
            CultureInfo userPreferredCulture = CultureInfo.CurrentCulture;

            switch (userPreferredCulture.Name)
            {
                case "en":
                case "en-GB":
                case "en-US":
                    Diagnostics.LogMessage("Language Detected: English");
                    CurrentLanguage = Language.English;
                    break;
                case "es":
                case "es-ES":
                case "es-US":
                case "es-MX":
                    Diagnostics.LogMessage("Language Detected: Spanish");
                    CurrentLanguage = Language.Spanish;
                    break;
                case "fr":
                case "fr-FR":
                case "fr-CA":
                    Diagnostics.LogMessage("Language Detected: French");
                    CurrentLanguage = Language.French;
                    break;
                case "it":
                case "it-it":
                    Diagnostics.LogMessage("Language Detected: Italian");
                    CurrentLanguage = Language.Italian;
                    break;
                case "ja":
                    Diagnostics.LogMessage("Language Detected: Japanese");
                    CurrentLanguage = Language.Japanese;
                    break;
                case "ko":
                    Diagnostics.LogMessage("Language Detected: Korean");
                    CurrentLanguage = Language.Korean;
                    break;
                case "pt-BR":
                case "pt-PT":
                    Diagnostics.LogMessage("Language Detected: Portuguese");
                    CurrentLanguage = Language.Portuguese;
                    break;
                case "ru":
                    Diagnostics.LogMessage("Language Detected: Russian");
                    CurrentLanguage = Language.Russian;
                    break;
                case "zh-Hans":
                    Diagnostics.LogMessage("Language Detected: Chinese - Simplified");
                    CurrentLanguage = Language.ChineseSimplified;
                    break;
                case "zh-Hant":
                    Diagnostics.LogMessage("Language Detected: Chinese - Traditional");
                    CurrentLanguage = Language.ChineseTraditional;
                    break;
                default:
                    Diagnostics.LogMessage($"Culture {userPreferredCulture.Name} not recognized, defaulting to English");
                    CurrentLanguage = Language.English;
                    break;
            }
        }

        private static void OnLanguageChanged(EventArgs e, Language language)
        {
            LanguageSwitched?.Invoke(e, language);
        }
    }

    public enum Language
    {
        None = 0,
        English = 1,
        Spanish = 2,
        French = 3,
        Italian = 4,
        Japanese = 5,
        Korean = 6,
        Portuguese = 7,
        Russian = 8,
        ChineseSimplified = 9,
        ChineseTraditional = 10,
    }
}

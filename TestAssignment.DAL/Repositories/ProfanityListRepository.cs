using LazyCache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using TestAssignment.DAL.Helpers;
using TestAssignment.DAL.Repositories.Interfaces;

namespace TestAssignment.DAL.Repositories
{
    /***
     * Currently a json file is used to store the profanity words
     * Lazy cache nuget is used to store the profanity list in memory.
     * ***/
    public class ProfanityListRepository : IProfanityListRepository
    {
        private readonly ILogger<ProfanityListRepository> _logger;
        private IAppCache _cache;
        private readonly string _jsonFile;

        public ProfanityListRepository(ILogger<ProfanityListRepository> logger, IAppCache cache)
        {
            _logger = logger;
            _cache = cache;
            _jsonFile = ProfanityFileHelper.BannedWordsPath(_logger);
        }

        public async Task<bool> AddProfanity(string profanity)
        {
            var jsonObject = await GetProfanityJObject();
            bool profanityAdded = false;

            if (jsonObject != null)
            {
                var bannedList = await GetProfanityList();
                if (bannedList.Contains(profanity))
                {
                    _logger.LogWarning($"Profanity List already contains {profanity}");
                }
                else
                {
                    bannedList.Add(profanity);
                    jsonObject["BannedWords"] = JArray.FromObject(bannedList);
                    string addedProfanityList = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    _logger.LogInformation($"Profanity added to the profanity list");
                    profanityAdded = await UpdateProfanityFile(addedProfanityList);
                    if (profanityAdded)
                    {
                        _cache.Remove("BannedWordsList");
                        _logger.LogInformation("Profanity List removed from cache.");
                    }
                }
            }
            return profanityAdded;
        }

        public async Task<bool> DeleteProfanity(string profanity)
        {
            var jsonObject = await GetProfanityJObject();

            bool profanityDeleted = false;

            if (jsonObject != null)
            {
                var bannedList = await GetProfanityList();
                if (bannedList.Remove(profanity))
                {
                    jsonObject["BannedWords"] = JArray.FromObject(bannedList);
                    string removedProfanityList = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    _logger.LogInformation($"Profanity deleted from ProfanityList.");
                    profanityDeleted = await UpdateProfanityFile(removedProfanityList);

                    if (profanityDeleted)
                    {
                        _cache.Remove("BannedWordsList");
                        _logger.LogInformation("Profanity List removed from cache.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Profanity does not exists in ProfanityList.");
                }
            }
            return profanityDeleted;
        }

        public async Task<List<string>> GetProfanityList()
        {
            // define a func to get the Banned Words List but do not Execute() it
            Func<Task<List<string>>> cachableBannedWordsListFunc = () => ExtractProfanityList();

            //Change the cache duration with absolute expiration
            var bannedWordsListWithCaching = await _cache.GetOrAddAsync("BannedWordsList", cachableBannedWordsListFunc, DateTimeOffset.Now.AddHours(2));

            return bannedWordsListWithCaching;
        }

        internal async Task<JObject> GetProfanityJObject()
        {
            // define a func to get the Json File Object Func but do not Execute() it
            Func<Task<JObject>> cachableJsonFileObjectFunc = () => ExtractProfanityJObject();

            //Change the cache duration with absolute expiration
            var jsonFileObjectWithCaching = await _cache.GetOrAddAsync("JsonFileObject", cachableJsonFileObjectFunc, DateTimeOffset.Now.AddHours(2));

            return jsonFileObjectWithCaching;

        }

        internal async Task<JObject> ExtractProfanityJObject()
        {
            try
            {
                var json = await File.ReadAllTextAsync(_jsonFile);
                var jsonObject = JObject.Parse(json);
                _logger.LogInformation("Profanity JObject is Extracted.");
                return jsonObject;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Extracting the Profanity JObject.");
                throw ex;
            }
        }

        internal async Task<bool> UpdateProfanityFile(string jsonProfanityList)
        {
            bool profanityUpdated = false;
            try
            {
                await File.WriteAllTextAsync(_jsonFile, jsonProfanityList);
                _cache.Remove("JsonFileObject");
                _logger.LogInformation("Profanity Json File is updated successfully.");
                profanityUpdated = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Updating Profanity Json File.");
                throw ex;
            }
            return profanityUpdated;
        }
        internal async Task<List<string>> ExtractProfanityList()
        {
            List<string> bannedList = new List<string>();
            var jsonObject = await GetProfanityJObject();
            if (jsonObject != null)
            {
                JArray bannedArray = (JArray)jsonObject["BannedWords"];
                _logger.LogInformation($"Profanity List extracted from the Json File");
                bannedList = (List<string>)bannedArray.ToObject(typeof(List<string>));
            }
            return bannedList;
        }
    }
}

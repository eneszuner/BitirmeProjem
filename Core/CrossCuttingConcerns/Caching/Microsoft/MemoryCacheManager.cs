using Core.Utilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        IMemoryCache _memoryCache;

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
        }
        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public bool IsAdd(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _memoryCache?.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_memoryCache) as dynamic;
            List<ICacheEntry> cacheCollectionValues = new List<ICacheEntry>();

            foreach (var cacheItem in cacheEntriesCollection)
            {
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }

            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList();

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }
            //1:56:02
        }
    }
    /*Bir bulakatta olaya hakim birisi varsa bu tip sorular ve bunların varyasyonlarını sorar mesela burada direkt aspect'in
     * içinde adam zaten yazmış memorycache yazan microsft elemanı yani .NETCORE geliştiren arkadaşlar bunu yazmışlar yani
     * hepsinin karşılığı var ben niye gidipte böyle metotlar yapıp yapıp duruyorum ha buarada sektördeki baya bi kodda direkt 
     * bunları çağırmak üzerinedir ha ben nabiyorum ben gidiyorum bunu MemoryCacheManager diye birşey oluşturuyorum onun içine koyuyorum
     * bakın hep aslında aynı mantıktan gidiyoruz benim derdim sadece Microsoft'un MemoryCache'ne eklemek değil eğer onu eklersem
     * yarın öbür gün başka bir Cache yönetiminde patlarım çünkü ben ne yaptım hardcode gittim her yere şu kodları yazdım 
     * yani aspect'in içine de olur gittim hardcode bu kodları yazdım yazarsam yarın öbürgün farklı bir cache sisteminde ben patlarım
     * ben ne yapıyorum buraya dikkat ben ne yapıyorum aslında bu adamı MemoryCache kodunu yani .NETCORE dan gelen kodu kendime uyarlıyorum
     * burada şuan arkadaşlar biz çok kullanılan patternlardan Adapter Pattern yani adaptasyon desenini uyguladık yani bir şeyi kendi
     * sistemime uyarlıyorum var olan bir sistemi kendi sistemime uyarlıyorum
     */






}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace ChannelCategorizer
{
    class Program
    {
        static Dictionary<string, List<string>> CategorizeChannels(string filePath)
        {
            var channels = new Dictionary<string, List<string>>();
            
            try
            {
                using (var reader = new StreamReader(filePath, System.Text.Encoding.UTF8, true))
                {
                    string line;
                    int lineCount = 0;
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        lineCount++;
                        if (lineCount % 10000 == 0)
                            Console.WriteLine($"  {lineCount} satır okundu...");
                        
                        // tvg-name=" ile başlayan satırı bul
                        var match = Regex.Match(line, @"tvg-name=""([^""]+)""");
                        if (!match.Success) continue;
                        
                        string title = match.Groups[1].Value;
                        string category = CategorizeTitle(title);
                        
                        if (!channels.ContainsKey(category))
                            channels[category] = new List<string>();
                        
                        channels[category].Add(title);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hata: {ex.Message}");
                return null;
            }
            
            return channels;
        }
        
        static string CategorizeTitle(string title)
        {
            string lower = title.ToLower();
            
            // Dizi
            if (Regex.IsMatch(lower, @"(dizi|series|türk dizi|ask-i memnu|ezel|medcezir)"))
                return "📺 Dizi";
            
            // Film
            if (Regex.IsMatch(lower, @"(film|movie|cinema|sinema|4k movie|full hd film)"))
                return "🎬 Film";
            
            // Spor
            if (Regex.IsMatch(lower, @"(spor|sports|futbol|football|nba|nfl|f1|tennis|voleybol|basketball|cricket)"))
                return "⚽ Spor";
            
            // Haber
            if (Regex.IsMatch(lower, @"(haber|news|habertürk|cnn|bbc|skynews|ntv|fox)"))
                return "📰 Haber";
            
            // Müzik
            if (Regex.IsMatch(lower, @"(müzik|music|radyo|radio)"))
                return "🎵 Müzik";
            
            // Çocuk
            if (Regex.IsMatch(lower, @"(çocuk|kids|cartoon|babytv|nickelodeon|trt çocuk)"))
                return "🎨 Çocuk";
            
            // Belgesel
            if (Regex.IsMatch(lower, @"(belgesel|documentary|discovery|nat geo|history)"))
                return "🎬 Belgesel";
            
            // Yetişkin
            if (Regex.IsMatch(lower, @"(adult|xxx|erotik|18\+)"))
                return "⚠️  Yetişkin";
            
            // Ülke kodları
            var codeMatch = Regex.Match(title, @"\[([A-Z]{2})\]");
            if (codeMatch.Success)
            {
                string code = codeMatch.Groups[1].Value;
                return code switch
                {
                    "TR" => "🇹🇷 Türkiye",
                    "EN" or "GB" or "DE" or "FR" or "IT" or "ES" or "NL" or "BE" or "AT" or "CH" => "🌍 Avrupa",
                    "RS" or "BG" or "GR" or "XK" or "BA" or "HR" or "ME" or "RO" or "UA" => "🇷🇸 Balkanlar",
                    "AE" or "SA" or "EG" or "IQ" or "IL" or "LB" or "JO" => "🕌 Ortadoğu",
                    "CN" or "IN" or "JP" or "KR" or "TH" or "ID" or "MY" or "VN" => "🏯 Asya",
                    "BR" or "AR" or "MX" or "CO" or "CL" => "🌎 Amerika",
                    _ => $"🌐 {code}"
                };
            }
            
            // Türkiye yazısı varsa
            if (Regex.IsMatch(lower, @"(turkey|türk|tr:|turkiye)"))
                return "🇹🇷 Türkiye";
            
            return "🔹 Diğer";
        }
        
        static void Main(string[] args)
        {
            string inputFile = @"C:\Users\bayin\Downloads\TV Channels.txt";
            string outputFile = @"C:\Users\bayin\OneDrive\Masaüstü\IPTV\kanal_kategorileri.txt";
            
            Console.WriteLine("📂 Kanallar kategorileştiriliyor...\n");
            
            var channels = CategorizeChannels(inputFile);
            
            if (channels == null)
                return;
            
            // Kategorileri kanal sayısına göre sırala
            var sorted = channels.OrderByDescending(x => x.Value.Count).ToList();
            
            // Konsola yazdır
            Console.WriteLine($"\n✅ Kategorileştirme tamamlandı\n");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("📊 KANAL KATEGORİZASYONU");
            Console.WriteLine("═══════════════════════════════════════════════════════════════\n");
            
            int totalChannels = 0;
            foreach (var cat in sorted)
            {
                int count = cat.Value.Count;
                totalChannels += count;
                
                Console.WriteLine($"{cat.Key}: {count} kanal");
                
                // İlk 3 kanalı göster
                for (int i = 0; i < Math.Min(3, count); i++)
                {
                    Console.WriteLine($"  {i+1}. {cat.Value[i]}");
                }
                
                if (count > 3)
                    Console.WriteLine($"  ... ve {count - 3} kanal daha\n");
            }
            
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"📊 TOPLAM: {totalChannels} kanal, {sorted.Count} kategori");
            Console.WriteLine("═══════════════════════════════════════════════════════════════\n");
            
            // Dosyaya yaz
            try
            {
                using (var writer = new StreamWriter(outputFile, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("════════════════════════════════════════════════════════════════");
                    writer.WriteLine("KANAL KATEGORİZASYONU RAPORU");
                    writer.WriteLine("════════════════════════════════════════════════════════════════\n");
                    
                    foreach (var cat in sorted)
                    {
                        writer.WriteLine($"\n{new string('═', 70)}");
                        writer.WriteLine($"{cat.Key.ToUpper()} ({cat.Value.Count} kanal)");
                        writer.WriteLine(new string('═', 70) + "\n");
                        
                        for (int i = 0; i < cat.Value.Count; i++)
                        {
                            writer.WriteLine($"{i+1}. {cat.Value[i]}");
                        }
                    }
                    
                    writer.WriteLine($"\n\n════════════════════════════════════════════════════════════════");
                    writer.WriteLine($"ÖZET: {totalChannels} kanal, {sorted.Count} kategori");
                    writer.WriteLine($"════════════════════════════════════════════════════════════════");
                }
                
                Console.WriteLine($"✅ Sonuçlar kaydedildi: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Dosya yazma hatası: {ex.Message}");
            }
        }
    }
}

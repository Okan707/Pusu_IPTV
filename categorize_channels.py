#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import re
from collections import defaultdict
from pathlib import Path

def extract_channel_info(line):
    """EXTINF satırından channel bilgisini çıkar"""
    # tvg-name=" ile başlayan ve sonraki tırnak arasındaki metni al
    match = re.search(r'tvg-name="([^"]+)"', line)
    if match:
        title = match.group(1)
        return title
    return None

def categorize_by_title(title):
    """Title'a göre kategori belirle"""
    title_lower = title.lower().strip()
    
    # Dizi kategorileri
    if any(x in title_lower for x in ['dizi', 'series', 'türk dizi', 'turkish series', 'ask-i memnu', 'ezel', 'medcezir']):
        return 'Dizi'
    
    # Film kategorileri
    if any(x in title_lower for x in ['film', 'movie', 'cinema', 'sinema', '4k movie', 'full hd film']):
        return 'Film'
    
    # Spor kategorileri
    if any(x in title_lower for x in ['spor', 'sports', 'futbol', 'football', 'nba', 'nfl', 'f1', 'tennis', 'voleybol']):
        return 'Spor'
    
    # Haber kategorileri
    if any(x in title_lower for x in ['haber', 'news', 'habertürk', 'cnn', 'bbc', 'skynews']):
        return 'Haber'
    
    # Müzik kategorileri
    if any(x in title_lower for x in ['müzik', 'music', 'radyo', 'radio']):
        return 'Müzik'
    
    # Çocuk kategorileri
    if any(x in title_lower for x in ['çocuk', 'kids', 'cartoon', 'babytv', 'nickelodeon', 'trt çocuk']):
        return 'Çocuk'
    
    # Belgesel kategorileri
    if any(x in title_lower for x in ['belgesel', 'documentary', 'discovery', 'nat geo', 'history']):
        return 'Belgesel'
    
    # Erotik kategorileri
    if any(x in title_lower for x in ['adult', 'xxx', 'erotik', '18+']):
        return 'Yetişkin'
    
    # Yabancı dil kanalları (başında flag/ülke kodu varsa)
    if title.startswith('['):
        match = re.match(r'\[([A-Z]{2})\]', title)
        if match:
            country_code = match.group(1)
            # Türkiye
            if country_code == 'TR':
                return 'Türkiye'
            # Avrupa
            elif country_code in ['EN', 'GB', 'DE', 'FR', 'IT', 'ES']:
                return 'Avrupa'
            # Balkanlar
            elif country_code in ['RS', 'RS', 'BG', 'GR', 'XK', 'BA', 'HR', 'ME']:
                return 'Balkanlar'
            # Ortadoğu
            elif country_code in ['AE', 'SA', 'EG', 'IQ', 'IL', 'TR', 'LB']:
                return 'Ortadoğu'
            # Asya
            elif country_code in ['CN', 'IN', 'JP', 'KR', 'TH', 'ID', 'MY']:
                return 'Asya'
            else:
                return f'Ülke: {country_code}'
    
    # Türkiye yazısı varsa
    if 'turkey' in title_lower or 'türk' in title_lower or 'tr:' in title_lower:
        return 'Türkiye'
    
    # Varsayılan
    return 'Diğer'

def process_m3u_file(input_file):
    """M3U dosyasını oku ve kategorize et"""
    channels = defaultdict(list)
    
    try:
        with open(input_file, 'r', encoding='utf-8', errors='ignore') as f:
            for line in f:
                line = line.strip()
                
                # EXTINF satırını bul
                if line.startswith('#EXTINF'):
                    title = extract_channel_info(line)
                    if title:
                        category = categorize_by_title(title)
                        channels[category].append(title)
    except Exception as e:
        print(f"Hata: {e}")
        return None
    
    return channels

def main():
    input_file = Path('C:/Users/bayin/Downloads/TV Channels.txt')
    output_file = Path('C:/Users/bayin/OneDrive/Masaüstü/IPTV/kanal_kategorileri.txt')
    
    print(f"📂 Dosya okunuyor: {input_file}")
    channels = process_m3u_file(input_file)
    
    if not channels:
        print("❌ Dosya işlenemiyor!")
        return
    
    # Sonuçları sırala
    sorted_categories = sorted(channels.items(), key=lambda x: len(x[1]), reverse=True)
    
    # Konsola yazdır
    print(f"\n✅ Toplam {sum(len(v) for v in channels.values())} kanal bulundu\n")
    print("=" * 80)
    
    total_channels = 0
    for category, titles in sorted_categories:
        count = len(titles)
        total_channels += count
        print(f"\n📺 {category}: {count} kanal")
        print("-" * 80)
        
        # İlk 5 kanalı göster
        for i, title in enumerate(titles[:5], 1):
            print(f"  {i}. {title}")
        
        if len(titles) > 5:
            print(f"  ... ve {len(titles) - 5} kanal daha")
    
    # Dosyaya da yaz
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("KANAL KATEGORİZASYONU\n")
        f.write("=" * 80 + "\n\n")
        
        for category, titles in sorted_categories:
            count = len(titles)
            f.write(f"\n{'='*80}\n")
            f.write(f"📺 {category.upper()} ({count} kanal)\n")
            f.write(f"{'='*80}\n\n")
            
            for i, title in enumerate(titles, 1):
                f.write(f"{i}. {title}\n")
    
    print(f"\n\n✅ Sonuçlar kaydedildi: {output_file}")
    print(f"📊 Toplam: {total_channels} kanal, {len(channels)} kategori")

if __name__ == '__main__':
    main()

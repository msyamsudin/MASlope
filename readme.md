# MA Slope Indicator

Indikator cTrader yang menampilkan kemiringan Moving Average sebagai histogram berwarna.

## Fitur

- Histogram hijau (naik) / merah (turun)
- Threshold untuk filter sinyal
- Multiple data sources (Close, Open, High, Low, Median, Typical, Weighted)
- Smoothing untuk mengurangi noise
- Moving Average line untuk konfirmasi trend
- Zero line reference

## Parameter

| Parameter | Default | Deskripsi |
|-----------|---------|-----------|
| Period | 14 | Periode Moving Average |
| MA Type | Simple | Jenis MA (SMA, EMA, WMA, dll) |
| Threshold (%) | 0.1 | Nilai minimum histogram penuh |
| Source | Close | Sumber data harga |
| Smoothing Period | 1 | Periode smoothing (1 = off) |
| Show MA Line | true | Tampilkan MA pada slope |
| MA Line Period | 14 | Periode MA line |
| MA Line Type | Simple | Jenis MA line |
| Show Zero Line | true | Tampilkan garis zero |

## Instalasi

1. Buka cTrader → Automate → Manage Indicators
2. Add Indicator → Copy paste kode
3. Build → Aktifkan

## Interpretasi

**Histogram:**
- Hijau penuh = Trend naik kuat (slope > threshold)
- Merah penuh = Trend turun kuat (slope < -threshold)
- Titik kecil = Trend lemah (slope < threshold)

**MA Line:**
- Naik = Bullish momentum
- Turun = Bearish momentum
- Flat = Sideways

## Strategi

### Trend Following
- **BUY**: Histogram hijau + MA line naik + slope > threshold
- **SELL**: Histogram merah + MA line turun + slope < -threshold

### Divergence
- **Bullish**: Harga lower low, slope higher low → reversal naik
- **Bearish**: Harga higher high, slope lower high → reversal turun

### Momentum
- **Strong**: Histogram tinggi + MA searah
- **Weak**: Histogram mengecil → persiapan exit

## Rekomendasi Setting

| Timeframe | Period | Threshold | Smoothing | MA Line |
|-----------|--------|-----------|-----------|---------|
| **Scalping (M1-M5)** | 5-10 | 0.05-0.1% | 2-3 | 5-7 |
| **Day Trading (M15-H1)** | 14-20 | 0.1-0.2% | 1-2 | 14-20 |
| **Swing Trading (H4-D1)** | 20-50 | 0.2-0.5% | 3-5 | 20-30 |

## Kombinasi Indikator

- **RSI**: Konfirmasi overbought/oversold
- **MACD**: Double konfirmasi trend
- **S/R**: Validasi breakout
- **Volume**: Konfirmasi kekuatan trend

## Tips

✅ **Do:**
- Sesuaikan threshold dengan volatilitas
- Kombinasikan dengan price action
- Test di demo account
- Gunakan risk management

❌ **Don't:**
- Trading hanya dari 1 indikator
- Threshold terlalu rendah (noise)
- Abaikan fundamental
- Over-optimize

## Troubleshooting

| Masalah | Solusi |
|---------|--------|
| Histogram tidak muncul | Kurangi threshold / Period |
| Terlalu banyak sinyal | Naikkan threshold + smoothing |
| Sinyal terlambat | Kurangi Period / Smoothing |

---

**Disclaimer**: Trading memiliki risiko tinggi. Indikator ini hanya alat bantu analisis, bukan jaminan profit.
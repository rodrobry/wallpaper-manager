# Wallpaper Randomizer
A lightweight C# utility to randomize wallpapers across multi-monitor setups on demand.

## Overview
Unlike background swappers that constantly poll in the background, Wallpaper Randomizer runs strictly when triggered:
- **Zero Performance Overhead:** Prevents unexpected resource spikes or lag during heavy workloads and gaming (ideal for lower-end machines).
- **Full Visual Control:** Change wallpapers only when you want to, cycling until you hit a combination you like.

## Wallpaper Folder Structure
The app categorizes wallpapers by **orientation** within any number of **category/topic** folders.
The 'BaseFolder' defaults to 'C:\Users\<Username>\Pictures\wallpapers', but can be customized through the **config.json** file.
Example:

```text
<BaseFolder>/
├── gta6/
│   ├── horizontal/
│   └── vertical/
├── vRising/
│   ├── horizontal/
│   └── vertical/
│   ...
└── categoryN/
    ├── horizontal/
    └── vertical/

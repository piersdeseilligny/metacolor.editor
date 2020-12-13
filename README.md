<img src="/Metacolor.Editor/Assets/icon.png" width="256">

## What


This is a tool built in .net to view and edit the metadata embedded in ProRes and/or Quicktime video files, on a frame-by-frame basis, via a simple UI. It can be compared to the BBC's [qtff-parameter-editor](https://github.com/bbc/qtff-parameter-editor) tool, although metacolor.editor uses a different approach behind the scenes, and significantly improves the user experience.

It is capable of modifying the colour-related metadata on any Quicktime file, regardless of codec, and on any Prores video, regardless of the container.


![Screenshots](https://i.imgur.com/pa4ahVC.png)

## Why
The Quicktime container and Prores codec are capable of specifying the color primary, transfer function, and color matrix to be used when viewing the file. However this information may be incorrect for a variety of reasons (Generally the tool used to encode the file was unaware or incapable of producing this data). This can result in ProRes videos being played back with inaccurate colors or post-production tools incorrectly transcoding them.

**This tool allows you to view, analyse, and modify this metadata.**


## How

It does so with a naive byte-level analysis, which means that this application does not require the use of any external tools: It is fully self-contained. This also makes it extremely versatile, as it can open and modify any file containing ProRes video data. It is compatible with any past, present, or future ProRes container, as well as ProRes video contained in an unorthodox or undocumented way (use at your own risk). 

The tool performs a thorough analysis of the file when it is first opened, finding the offset of every prores frame, and the QuickTime `colr` atom, if applicable. This may take a few seconds, depending on the size of the file and your configuration, however the replacement of the metadata is then near-instant. It's also possible to export this analysis as a JSON file, for use in other or custom tools.

# Features

* Replacing the color primary, transfer function, and color matrix in the quicktime `colr` atom and in every single prores frame

* Replacing the "Creator ID" in the ProRes frames

* Examining and comparing all available frame metadata in ProRes video.

* Warning of any inconsistencies in colour metadata (This is what the "Errors" tab is for)

* Batch processing multiple files


# Download

**THIS SHOULDN'T CORRUPT YOUR PRORES FILES, BUT PLEASE USE AT YOUR OWN RISK!**

## Windows

**[Download from the Microsoft Store (recommended)](https://www.microsoft.com/store/productId/9PLK5VZS2QN8)**

**[Download .zip for Windows](https://github.com/piersdeseilligny/prores.editor/releases/download/v1.0/Windows.zip)**

## macOS

**[Download .app for Mac](https://github.com/piersdeseilligny/prores.editor/releases/download/v1.0/Mac.zip)**

## Debian


*Please note that this tool has not been tested extensively on Mac. But it should work.*

Built with [Eto](https://github.com/picoe/Eto).

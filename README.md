# ProRes.Editor

## What

This is a tool built in .net to view and edit the metadata embedded in ProRes videos, on a frame-by-frame basis, via a simple UI.


![Screenshots](https://i.imgur.com/pa4ahVC.png)

## Why
ProRes is capable of specifying the color primary, transfer function, and color matrix to be used when viewing the file. However this information may be incorrect for a variety of reasons (Generally the tool used to encode the file was unaware or incapable of producing this data). This can result in ProRes videos being played back with inaccurate colors or post-production tools incorrectly transcoding them.

**This tool allows you to view and modify this metadata.**


## How

It does so with a naive byte-level analysis, which means that this application does not require the use of any external tools: It is fully self-contained. This also makes it extremely versatile, as it can open and modify any file containing ProRes video data. It is compatible with any past, present, or future ProRes container, as well as ProRes video contained in an unorthodox or undocumented way (use at your own risk). 

The tool performs a thorough analysis of the file when it is first opened, finding the offset of every prores frame, and the QuickTime `colr` atom, if applicable. This may take a few seconds, depending on the size of the file and your configuration, however the replacement of the metadata is then near-instant.


# Download

**[Download portable .zip for Windows]()**

**[Download .app for Mac]()**

*Please note that this tool has not been tested extensively on Mac. But it should work.*

Built with [Eto](https://github.com/picoe/Eto).
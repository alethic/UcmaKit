# UcmaKit
Various utilities for building UCMA applications.

## UcmaKit.Rtc
Provides a common application and configuration model for UCMA services. Specific services inherit from RtcApplication
which provides a variety of methods that can be implemented to handle incoming calls. This project takes care of all
the underlying configuration of setting up a CollaborationPlatform, whether automatically provisioned (Skype for
Business Server) or manually.

## UcmaKit.Rtc.Acd
Beginnings of an automatic call distribution system. This is not yet finished.

## UcmaKit.Rtc.Queue
Beginnings of a call queue system. This is not yet finished.

## UcmaKit.Rtc.VoiceXml
UcmaKit application that answers calls, looks up call information in a SQL server, and directs the call to a VoiceXML URL.

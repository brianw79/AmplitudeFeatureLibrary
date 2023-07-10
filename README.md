# AmplitudeFeatureLibrary
Simple C# library for retrieving Amplitude feature flags

### Usage
```
var amplitude = new AmplitudeFeature("Your_API_key");
var enabled = amplitude.FeatureIsEnabled("the_flag_name");
```

# Solution Information
The Amplitude Feature Library solution contains the following 3 projects:

### Amplitude Library
A small class library project for interacting with Amplitude feature flags
### Amplitude Library Tests
Unit tests for the Amplitude feature library project
### Amplitude Library Console Tester
A small console project that allows you to see the library code in action

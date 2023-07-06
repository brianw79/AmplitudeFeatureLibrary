# AmplitudeFeatureLibrary
Simple C# library for retrieving Amplitude feature flags

### Usage
```
var amplitude = new AmplitudeFeature("Your_API_key");
var enabled = amplitude.FeatureIsEnabled("the_flag_name");
```

# StretchSense Open SDK OSC Integration Sample #

## Installation ##

Add the following packages via UPM:

### Open Sound Control Core / OscCore ###

[Open Sound Control Core](https://github.com/virtual-cast/OscCore)

1. Add this URL [https://github.com/virtual-cast/OscCore.git] via *Window -> Package Manager -> Add package from Git URL...*.
2. Optionally install the *Basic Examples* package for `OscCore` from the *Samples* tab in the Unity Package Manager.

Add the following optional packages via UPM:

### UniVRM ###

[UniVRM](https://github.com/vrm-c/UniVRM)

Install this package if you are planning to use VRM models in your Unity project. In the Hand Engine settings you must select the VRM retargeting preset under the *Open SDK* section.

1. Install the [latest release of UniVRM](https://github.com/vrm-c/UniVRM/releases/latest) by following the installation instructions. We recommend adding the listed packages in their instructions via their UPM URLs using *Window -> Package Manager -> Add package from Git URL...*.

2. After installation, any VRM models from the Models folder in this sample will automatically be converted into Unity prefabs which you can place in your scene. They will have their shaders and other supporting scripts added. This sample has the following VRM 1.0 models pre-configured in the *StretchSense Open SDK Studio Demo* scene:

- [AvatarSample_A](https://hub.vroid.com/en/characters/2843975675147313744/models/5644550979324015604)
- [AvatarSample_B](https://hub.vroid.com/en/characters/7939147878897061040/models/2292219474373673889)
- [AvatarSample_C](https://hub.vroid.com/en/characters/1248981995540129234/models/8640547963669442173)

These models are already set up with weight maps which are compatible with UniVRM's *VRM Spring Bone* component added to the `secondary` GameObject in each VRM model prefab. You can adjust these or enable/disable them depending on which body parts or clothing you want physics to be enabled on.

VRM 0.x models are also supported by UniVRM.

3. If you are planning on using the VRM model as an avatar that is part of a VR application, you can adjust the *Renderers* settings in the *VRM First Person* component to hide the Face, Body and Hair in first person mode so these do not appear in your view. [See more about the VRMFirstPerson feature here](https://vrm.dev/en/univrm/firstperson/univrm_firstperson/).

## Receiving OSC Messages Over Network ##

Hand Engine is configured to broadcast OSC messages via UDP on your network by default. This will make it possible to receive Open SDK data on other devices, in addition to the same PC running Hand Engine. Overall animation performance of the data will depend on the speed and configuration of your network hardware.

- Ensure that port `9400` (or other ports are not blocked by your network firewall) and that your network has no traffic shaping policies for broadcast packets on this port. Some routers may block or limit broadcast packets by default, so you may need to disable this limit in your router's control panel.
- If you have performance issues with the `animation/rotation`, `animation/position` and `animation/sliders` OSC addresses, set the *Streaming IP Address* to your target device's IP that will receive animation data (e.g. the IP of a Meta Quest headset).
- If your device is a standalone headset (e.g. Meta Quest), then set the following permissions in your `AndroidManifest.xml` file and perform a clean build:
```
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```
- If the PC running Hand Engine does not have a dedicated GPU, make sure Hand Engine is minimised while using the Open SDK to reduce the CPU load as it will not need to render the 3D animated hands on screen.

### Receiving OSC Messages to Meta Quest Headsets ###
1. If using Unity and a Meta Quest headset, ensure that *Project Setting -> XR Plug-in Management -> Open XR -> Android Tab -> Open XR Feature Groups -> Meta XR -> Meta Quest Support -> Configure -> Feature Settings -> Force Remove Internet* is unchecked, otherwise it will remove the above permissions and connections to Hand Engine via Open SDK will be blocked. 
2. Run *File -> Build Settings -> Android -> Build -> Clean Build...* to ensure the `AndroidManifest.xml` is updated correctly with the above change.

## Debugging OSC Messages ##

When using OscCore, you need to know the type of OSC message attributes relative to their indexes in order to correctly parse them. You can use the following tools to help with this:

- [OscCore](https://github.com/virtual-cast/OscCore) - The OscCore Unity package has a built in message debugger accessible from *Window -> OscCore -> Monitor*. This works for the OSC server created by your app if you are using the OscCore library.
- [osc-debugger](https://github.com/alexanderwallin/osc-debugger) - Node.js command line tool to connect OSC client and receive messages without knowing the address or message format.
- [extOSC](https://github.com/Iam1337/extOSC) - Unity plugin to debug OSC messages including various features to send/receive OSC messages.
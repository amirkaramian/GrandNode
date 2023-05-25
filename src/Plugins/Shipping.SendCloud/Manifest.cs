using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Shipping.SendCloud;

[assembly: PluginInfo(
    FriendlyName = "Shipping by SendCloud",
    Group = "Shipping rate",
    SystemName = SendCloudShippingDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "Revotec",
    Version = "1.00"
)]
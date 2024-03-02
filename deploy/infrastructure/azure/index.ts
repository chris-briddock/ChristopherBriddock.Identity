import * as azureNative from "@pulumi/azure-native";
import * as pulumi from '@pulumi/pulumi';

// Get the project and location from the Pulumi config
const config = new pulumi.Config();
const location = config.require("region");
const rgName = config.require("rgName");
const clusterName = config.require("clusterName");

// Create an Azure Resource Group
const resourceGroup = new azureNative.resources.ResourceGroup(rgName, {
    location: location,
});

// Deploy an AKS cluster
const cluster = new azureNative.containerservice.ManagedCluster(clusterName, {
    resourceGroupName: resourceGroup.name,
    location: resourceGroup.location,
    dnsPrefix: "akskubedns",
    agentPoolProfiles: [{
        count: 3,
        vmSize: "Standard_DS2_v2",
        name: "agentpool",
        mode: "System",
    }],
    identity: {
        type: "SystemAssigned",
    },
    kubernetesVersion: "1.21.2",
});
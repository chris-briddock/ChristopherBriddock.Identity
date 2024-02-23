import * as gcp from '@pulumi/gcp';
import * as pulumi from '@pulumi/pulumi';

// Get the project and location from the Pulumi config
const config = new pulumi.Config();
const project = config.require("project");
const location = config.require("region");

// Create a new VPC network
const network = new gcp.compute.Network("network", {
    project: project,
    autoCreateSubnetworks: true, // When set to true, the network is created with a single automatically created subnetwork per region.
});

// Define a service account for the cluster
const clusterServiceAccount = new gcp.serviceaccount.Account('cluster-sa', {
    accountId: 'autopilot-sa',
    displayName: 'GKE Autopilot service account',
});


// Create a GKE cluster with Autopilot mode enabled
const clusterAutopilot = new gcp.container.Cluster("clusterAutopilot", {
    name: "autopilot-cluster",
    location: location,
    network: network.selfLink,
    initialNodeCount: 1,
    enableAutopilot: true,
}, { dependsOn: [network, clusterServiceAccount] });

// Export the cluster endpoint
export const clusterEndpoint = clusterAutopilot.endpoint;

// Export the Cluster name
export const clusterName = clusterAutopilot.name;
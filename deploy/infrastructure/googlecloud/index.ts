import { Network } from '@pulumi/gcp/compute';
import { Cluster } from '@pulumi/gcp/container';
import { Account } from '@pulumi/gcp/serviceaccount';
import { Config } from '@pulumi/pulumi';

// Get the project and location from the Pulumi config
const config: Config = new Config();
const project: string = config.require("project");
const location: string = config.require("region");
const name: string = config.require("clusterName");

// Create a new VPC network
const network: Network = new Network("network", {
    project: project,
    autoCreateSubnetworks: true, // When set to true, the network is created with a single automatically created subnetwork per region.
});

// Define a service account for the cluster
const clusterServiceAccount: Account = new Account('cluster-sa', {
    accountId: 'autopilot-sa',
    displayName: 'GKE Autopilot service account',
});


// Create a GKE cluster with Autopilot mode enabled
const clusterAutopilot: Cluster = new Cluster(name, {
    name: name,
    location: location,
    network: network.selfLink,
    initialNodeCount: 1,
    enableAutopilot: true,
}, { dependsOn: [network, clusterServiceAccount] });

// Export the cluster endpoint
export const clusterEndpoint = clusterAutopilot.endpoint;

// Export the Cluster name
export const clusterName = clusterAutopilot.name;
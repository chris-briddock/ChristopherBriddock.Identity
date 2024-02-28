import * as aws from "@pulumi/aws";
import * as eks from "@pulumi/eks";
import * as awsx from "@pulumi/awsx";

// Create a VPC for our cluster
const vpc = new awsx.ec2.Vpc("vpc", {});

// Create an EKS cluster with the default configuration
const cluster = new eks.Cluster("cluster", {
    vpcId: vpc.vpcId,
    subnetIds: vpc.publicSubnetIds,
    instanceType: "t2.medium",
    desiredCapacity: 2,
    minSize: 1,
    maxSize: 2,
});


// Define an autoscaling policy for CPU utilization
const cpuScaleUpPolicy = new aws.autoscaling.Policy("cpuScaleUpPolicy", {
    autoscalingGroupName: "my-autoscaling-group",
    adjustmentType: "ChangeInCapacity",
    scalingAdjustment: 2,
    cooldown: 300,
    policyType: "TargetTrackingScaling",
    targetTrackingConfiguration: {
        predefinedMetricSpecification: {
            predefinedMetricType: "ASGAverageCPUUtilization",
        },
        targetValue: 50.0,
    },
});

// Define an autoscaling policy for CPU utilization to scale down
const cpuScaleDownPolicy = new aws.autoscaling.Policy("cpuScaleDownPolicy", {
    autoscalingGroupName: "my-autoscaling-group",
    adjustmentType: "ChangeInCapacity",
    scalingAdjustment: -1,
    cooldown: 300,
    policyType: "TargetTrackingScaling",
    targetTrackingConfiguration: {
        predefinedMetricSpecification: {
            predefinedMetricType: "ASGAverageCPUUtilization",
        },
        targetValue: 50.0,
        disableScaleIn: false,
    },
});

// Output the ARN of the scale-up policy
export const scaleUpPolicyArn = cpuScaleUpPolicy.arn;

// Output the ARN of the scale-down policy
export const scaleDownPolicyArn = cpuScaleDownPolicy.arn;
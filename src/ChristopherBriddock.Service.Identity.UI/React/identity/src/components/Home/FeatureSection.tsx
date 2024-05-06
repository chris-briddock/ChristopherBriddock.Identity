import React from "react";
import FeatureItem from "./FeatureItem";

const FeaturesSection: React.FC = () => {
    return (
      <section className="features bg-gray-100 py-20">
        <div className="container mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">Features</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <FeatureItem title="Authentication" description="Allow users to securely authenticate using various authentication methods." />
            <FeatureItem title="Authorization" description="Control access to resources based on user permissions and roles." />
            <FeatureItem title="Single Sign-On (SSO)" description="Enable users to access multiple applications with a single login." />
          </div>
        </div>
      </section>
    );
  };
  
  export default FeaturesSection;
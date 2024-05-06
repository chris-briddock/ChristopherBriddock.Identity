import React from "react";

const FeatureItem: React.FC<{ title: string; description: string }> = ({ title, description }) => {
    return (
      <div className="feature-item bg-white rounded-lg p-6 shadow-md">
        <h3 className="text-2xl font-semibold mb-4">{title}</h3>
        <p className="text-gray-700">{description}</p>
      </div>
    );
  };

export default FeatureItem;
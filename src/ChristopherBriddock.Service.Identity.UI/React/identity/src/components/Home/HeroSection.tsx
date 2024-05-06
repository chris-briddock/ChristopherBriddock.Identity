import Link from 'next/link';

const HeroSection: React.FC = () => {
  return (
    <section className="hero bg-gray-900 text-white py-20">
      <div className="container mx-auto text-center">
        <h1 className="text-5xl font-bold mb-4">Welcome!</h1>
        <p className="text-xl mb-6">Securely manage user identities and access control.</p>
        <Link className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded inline-block mt-4" href="/register"> 
            Register
        </Link>
      </div>
    </section>
  );
};

export default HeroSection;
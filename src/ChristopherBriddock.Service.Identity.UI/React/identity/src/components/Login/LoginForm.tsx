'use client'

import Link from 'next/link';
import React, { useState } from 'react';

const LoginComponent: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // Your submission logic here
    console.log('Form submitted with email:', email, 'and password:', password);
  };

  return (
    <form onSubmit={onSubmit}>
          <div className="flex justify-center items-center h-screen bg-gray-100">
      <div className="flex rounded-lg shadow-lg overflow-hidden">
        <div className="bg-black text-white px-12 py-16 flex flex-col justify-center">
          <h1 className="text-3xl font-bold mb-4">Welcome!</h1>
          <p className="text-gray-300">Use the form to sign in to your account.</p>
        </div>
        <div className="bg-white px-12 py-16">
          <h2 className="text-2xl font-bold mb-4">Login to your account</h2>
          <p className="text-gray-500 mb-6">
            Need an account? <Link href="#" className="text-blue-500">Sign up</Link></p>
          <div className="mb-4">
            <button className="w-full bg-white text-gray-700 font-semibold py-2 px-4 border border-gray-400 rounded-lg shadow-sm mb-2 flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" className="w-5 h-5 mr-2" viewBox="0 0 30 30" width="30px" height="30px">
                <path d="M 15.003906 3 C 8.3749062 3 3 8.373 3 15 C 3 21.627 8.3749062 27 15.003906 27 C 25.013906 27 27.269078 17.707 26.330078 13 L 25 13 L 22.732422 13 L 15 13 L 15 17 L 22.738281 17 C 21.848702 20.448251 18.725955 23 15 23 C 10.582 23 7 19.418 7 15 C 7 10.582 10.582 7 15 7 C 17.009 7 18.839141 7.74575 20.244141 8.96875 L 23.085938 6.1289062 C 20.951937 4.1849063 18.116906 3 15.003906 3 z"/>
              </svg>
              Sign in with Google
            </button>
            <button className="w-full bg-blue-600 text-white font-semibold py-2 px-4 rounded-lg shadow-sm flex items-center justify-center">
              <svg className="w-5 h-5 mr-2" viewBox="0 0 24 24">
                <svg xmlns="http://www.w3.org/2000/svg"  viewBox="0 0 60 60" width="30px" height="30px">
                  <path fill="#ffffff" d="M25,3C12.85,3,3,12.85,3,25c0,11.03,8.125,20.137,18.712,21.728V30.831h-5.443v-5.783h5.443v-3.848 c0-6.371,3.104-9.168,8.399-9.168c2.536,0,3.877,0.188,4.512,0.274v5.048h-3.612c-2.248,0-3.033,2.131-3.033,4.533v3.161h6.588 l-0.894,5.783h-5.694v15.944C38.716,45.318,47,36.137,47,25C47,12.85,37.15,3,25,3z"/>
                </svg>
              </svg>
              Sign in with Facebook
            </button>
          </div>
          <div className="mb-4">
            <input className="w-full px-4 py-2 border border-gray-400 rounded-lg shadow-sm mb-2" type="email" placeholder="Email" name="email" required/>
            <input className="w-full px-4 py-2 border border-gray-400 rounded-lg shadow-sm" type="password" placeholder="Password" name="password" required/>
          </div>
          <a href="#" className="text-blue-500 mb-4 inline-block">Forgot password?</a>
          <button type="submit" className="w-full bg-black text-white font-semibold py-2 px-4 rounded-lg shadow-sm">Login</button>
        </div>
      </div>
    </div>
    </form>
  );
}

export default LoginComponent;
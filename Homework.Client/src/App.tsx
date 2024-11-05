import React from 'react';
import NavBar from './components/NavBar';
import ProductGrid from './components/ProductGrid';

const App: React.FC = () => (
    <div className="app">
        <NavBar />
        <ProductGrid />
    </div>
);

export default App;

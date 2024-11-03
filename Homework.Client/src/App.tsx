import React from 'react';
import NavBar from './components/NavBar';
import ProductList from './components/ProductList';

const App: React.FC = () => (
    <div className="app">
        <NavBar />
        <ProductList />
    </div>
);

export default App;

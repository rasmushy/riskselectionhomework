import React from 'react';
import { Product } from '../interfaces/product';

interface ProductCardProps {
    product: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({ product }) => (
    <div className="product-card">
        <h3>{product.title}</h3>
        <div className="product-info">
            {product.brand && <span className="product-brand">{product.brand}</span>}
            <span className="product-price">{product.price} €</span>
        </div>
    </div>
);

export default ProductCard;

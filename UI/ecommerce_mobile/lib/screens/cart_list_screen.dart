import 'package:ecommerce_mobile/providers/cart_provider.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class CartListScreen extends StatefulWidget {
  final VoidCallback? onGoToHome;

  const CartListScreen({super.key, this.onGoToHome});

  @override
  State<CartListScreen> createState() => _CartListScreenState();
}

class _CartListScreenState extends State<CartListScreen> {
  late CartProvider _cartProvider;

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _cartProvider = context.read<CartProvider>();

    // initData();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer(
      builder: (context, CartProvider cartProvider, child) {
        return SafeArea(
          child: Column(
            children: [
              _buildHeader(),
              Expanded(child: _buildItemList()),
            ],
          ),
        );
      },
    );
  }

  Widget _buildItemList() {
    if (_cartProvider.cart.items.isEmpty) {
      return _buildEmptyCart();
    }
    return ListView.builder(
      itemCount: _cartProvider.cart.items.length,
      itemBuilder: (context, index) {
        var cartItem = _cartProvider.cart.items[index];
        return ListTile(
          title: Text(cartItem.product.name ?? ""),
          subtitle: Text('Quantity: ${cartItem.quantity}'),
          trailing: IconButton(
            icon: Icon(Icons.remove_shopping_cart),
            onPressed: () {
              _cartProvider.removeFromCart(cartItem.product);
            },
          ),
        );
      },
    );
  }

  Widget _buildEmptyCart() {
    return GestureDetector(
      onTap: widget.onGoToHome,
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.shopping_cart_outlined, size: 80, color: Colors.grey),
            SizedBox(height: 16),
            Text(
              "Your cart is empty",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 8),
            Text(
              "Tap to browse products",
              style: TextStyle(fontSize: 16, color: Colors.grey),
            ),
            SizedBox(height: 24),
            ElevatedButton.icon(
              onPressed: widget.onGoToHome,
              icon: Icon(Icons.home),
              label: Text("Go to Home"),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Your Cart",
            style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 8),
          Text(
            "Review your selected items before checkout.",
            style: TextStyle(fontSize: 16, color: Colors.grey),
          ),
        ],
      ),
    );
  }
}

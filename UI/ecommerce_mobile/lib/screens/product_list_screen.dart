import 'package:ecommerce_mobile/layouts/master_screen.dart';
import 'package:flutter/material.dart';

class ProductListScreen extends StatefulWidget {
  const ProductListScreen({super.key});

  @override
  State<ProductListScreen> createState() => _ProductListScreenState();
}

class _ProductListScreenState extends State<ProductListScreen> {
  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: "Product List",
      child: Center(
        child: Text("TODO add product list screen"),
      ),
    );
  }
}
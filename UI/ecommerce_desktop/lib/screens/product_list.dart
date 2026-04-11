import 'package:ecommerce_desktop/layouts/master_screen.dart';
import 'package:ecommerce_desktop/models/product.dart';
import 'package:ecommerce_desktop/models/search_result.dart';
import 'package:ecommerce_desktop/providers/product_provider.dart';
import 'package:ecommerce_desktop/screens/product_details_screen.dart';
import 'package:ecommerce_desktop/utils/utils_widgets.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class ProductList extends StatefulWidget {
  const ProductList({super.key});

  @override
  State<ProductList> createState() => _ProductListState();
}

class _ProductListState extends State<ProductList> {
  late ProductProvider _productProvider;
  SearchResult<Product>? result;
  bool isLoading = true;

  TextEditingController _nameController = TextEditingController();

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _productProvider = context.read<ProductProvider>();

    initTable();
  }

  Future<void> initTable() async {
    try {
      var data = await _productProvider.get(filter: {});

      setState(() {
        result = data;
        isLoading = false;
      });
    } on Exception catch (e) {
      alertBox(context, 'Error', e.toString());
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: "Product List",
      child: Column(
        children: [
          _buildSearch(),
          isLoading ? CircularProgressIndicator() : _buildTable()
        ],
      ),
    );
  }

  Expanded _buildTable() {
    return Expanded(
          child: SizedBox(
            width: double.infinity,
            child: SingleChildScrollView(
              child: DataTable(
                columns: [
                  DataColumn(label: Text("Name")),
                  DataColumn(label: Text("Weight")),
                  DataColumn(label: Text("Price")),
                  DataColumn(label: Text("Product State")),
                ],
                  rows: result?.items
                  ?.map(
                    (e) => DataRow(
                      onSelectChanged: (value) {
                        Navigator.of(context)
                                .push(MaterialPageRoute(
                              builder: (context) => ProductDetailsScreen(product: e),
                            ));
                      },
                      cells: [
                      DataCell(Text(e.name ?? '')),
                      DataCell(Text(e.weight.toString())),
                      DataCell(Text(e.price.toString())),
                      DataCell(Text(e.productState ?? ''))
                    ]),
                  )
                  .toList() ?? List.empty(),
            ),
            ),
          ));
  }

  Padding _buildSearch() {
    return Padding(
          padding: const EdgeInsets.all(8.0),
          child: Row(
            children: [
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: TextField(
                    controller: _nameController,
                    decoration: InputDecoration(label: Text("Name")),
                  ),
                ),
              ),
              ElevatedButton(onPressed: () async{
                   try {
                        var data = await _productProvider.get(filter: {"name": _nameController.text});

                        setState(() {
                          result = data;
                          isLoading = false;
                        });
                      } on Exception catch (e) {
                        alertBox(context, 'Error', e.toString());
                      }
              }, child: Text("Search")),
              SizedBox(
                width: 10,
              ),
              ElevatedButton(onPressed: () async {
                var refresh = await Navigator.of(context).push(
                  MaterialPageRoute(
                    builder: (context) => const ProductDetailsScreen(
                      product: null,
                    )
                  )
                );

                if( refresh == "reload"){
                  initTable();
                }
              }, child: Text("New")),
            ],
          ),
        );
  }
}

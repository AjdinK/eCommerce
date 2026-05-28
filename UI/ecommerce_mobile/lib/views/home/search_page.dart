import 'dart:async';

import 'package:ecommerce_mobile/views/home/components/single_product_tile.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:provider/provider.dart';

import '../../core/components/app_back_button.dart';
import '../../core/constants/app_colors.dart';
import '../../core/constants/app_defaults.dart';
import '../../core/constants/app_icons.dart';
import '../../core/routes/app_routes.dart';
import '../../core/utils/ui_util.dart';
import '../../models/product.dart';
import '../../models/search_result.dart';
import '../../providers/product_provider.dart';
import '../../utils/utils_widgets.dart';
import 'dialogs/product_filters_dialog.dart';

class SearchPage extends StatefulWidget {
  const SearchPage({super.key});

  @override
  State<SearchPage> createState() => _SearchPageState();
}

class _SearchPageState extends State<SearchPage> {
  late ProductProvider _productProvider;

  SearchResult<Product>? productResult;
  bool isLoading = true;
  final TextEditingController _searchController = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  Timer? _searchDebounce;

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _productProvider = context.read<ProductProvider>();

    initData();
  }

  Future<void> initData() async {
    try {
      var data = await _productProvider.get(
        filter: {'name': _searchController.text.trim(), 'includeAssets': true},
      );

      setState(() {
        productResult = data;
        isLoading = false;
      });
    } on Exception catch (e) {
      alertBox(context, 'Error', e.toString());
    }
  }

  void _onSearchChanged(String value) {
    _searchDebounce?.cancel();
    _searchDebounce = Timer(const Duration(milliseconds: 400), () {
      initData();
    });
  }

  @override
  void dispose() {
    _searchDebounce?.cancel();
    _searchController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Column(
          children: [
            _SearchPageHeader(
              controller: _searchController,
              onChanged: _onSearchChanged,
            ),
            SizedBox(height: 8),
            isLoading || productResult == null || productResult!.items!.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : _RecentSearchList(productResult!.items!),
          ],
        ),
      ),
    );
  }
}

class _RecentSearchList extends StatelessWidget {
  final List<Product> products;

  const _RecentSearchList(this.products);

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDefaults.padding,
            ),
            child: Align(
              alignment: Alignment.centerLeft,
              child: Text(
                'Recent Search',
                style: Theme.of(
                  context,
                ).textTheme.bodyLarge?.copyWith(color: Colors.black),
              ),
            ),
          ),
          Expanded(
            child: ListView.separated(
              padding: const EdgeInsets.only(top: 16),
              itemBuilder: (context, index) {
                var product = products[index];

                return SingleProductTile(product: product);
              },
              separatorBuilder: (context, index) =>
                  const Divider(thickness: 0.1),
              itemCount: products.length,
            ),
          ),
        ],
      ),
    );
  }
}

class _SearchPageHeader extends StatelessWidget {
  final TextEditingController controller;
  final ValueChanged<String> onChanged;

  const _SearchPageHeader({required this.controller, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(AppDefaults.padding),
      child: Row(
        children: [
          const AppBackButton(),
          const SizedBox(width: 16),
          Expanded(
            child: Stack(
              children: [
                /// Search Box
                Form(
                  child: TextFormField(
                    controller: controller,
                    decoration: InputDecoration(
                      hintText: 'Search',
                      prefixIcon: Padding(
                        padding: const EdgeInsets.all(AppDefaults.padding),
                        child: SvgPicture.asset(
                          AppIcons.search,
                          colorFilter: const ColorFilter.mode(
                            AppColors.primary,
                            BlendMode.srcIn,
                          ),
                        ),
                      ),
                      prefixIconConstraints: const BoxConstraints(),
                      contentPadding: EdgeInsets.zero,
                      constraints: const BoxConstraints(),
                    ),
                    textInputAction: TextInputAction.search,
                    autofocus: true,
                    onChanged: onChanged,
                    onFieldSubmitted: (v) {
                      Navigator.pushNamed(context, AppRoutes.searchResult);
                    },
                  ),
                ),
                Positioned(
                  right: 0,
                  height: 56,
                  child: SizedBox(
                    width: 56,
                    child: ElevatedButton(
                      onPressed: () {
                        UiUtil.openBottomSheet(
                          context: context,
                          widget: const ProductFiltersDialog(),
                        );
                      },
                      style: ElevatedButton.styleFrom(
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                        ),
                      ),
                      child: SvgPicture.asset(AppIcons.filter),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class SearchHistoryTile extends StatelessWidget {
  const SearchHistoryTile({super.key});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () {},
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
        child: Row(
          children: [
            Text('Vegetables', style: Theme.of(context).textTheme.bodyMedium),
            const Spacer(),
            SvgPicture.asset(AppIcons.searchTileArrow),
          ],
        ),
      ),
    );
  }
}

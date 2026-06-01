import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/constants/constants.dart';
import '../../../models/cupon.dart';
import '../../../providers/cupon_provider.dart';
import '../../../utils/utils_widgets.dart';

class CouponCodeField extends StatefulWidget {
  final ValueChanged<Cupon?>? onCuponChanged;

  const CouponCodeField({super.key, this.onCuponChanged});

  @override
  State<CouponCodeField> createState() => _CouponCodeFieldState();
}

class _CouponCodeFieldState extends State<CouponCodeField> {
  late CuponProvider _cuponProvider;
  late TextEditingController controller;

  bool isFilled = false;

  Cupon? cupon;

  onChange(String? text) {
    if (text != null && text.isNotEmpty) {
      isFilled = true;
      setState(() {});
    } else {
      isFilled = false;
      setState(() {});
    }
  }

  @override
  void initState() {
    super.initState();
    controller = TextEditingController();
    _cuponProvider = context.read<CuponProvider>();
  }

  @override
  void dispose() {
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(AppDefaults.padding),
      child: Column(
        children: [
          Align(
            alignment: Alignment.centerLeft,
            child: Text(
              'Add Coupon',
              style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                fontWeight: FontWeight.bold,
                color: Colors.black,
              ),
            ),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: TextField(
                  decoration: const InputDecoration(
                    labelText: 'Entry Voucher Code',
                    contentPadding: EdgeInsets.symmetric(
                      horizontal: AppDefaults.padding,
                    ),
                  ),
                  onChanged: onChange,
                  controller: controller,
                ),
              ),
              const SizedBox(width: 16),
              SizedBox(
                width: MediaQuery.of(context).size.width * 0.3,
                child: ElevatedButton(
                  onPressed: () async {
                    try {
                      var result = await _cuponProvider.getByCode(
                        controller.text,
                      );
                      cupon = result;
                      widget.onCuponChanged?.call(cupon);
                    } on Exception catch (e) {
                      alertBox(context, "Error", e.toString());
                    }
                  },
                  style: ElevatedButton.styleFrom(
                    foregroundColor: isFilled ? null : AppColors.placeholder,
                    backgroundColor: isFilled
                        ? null
                        : Colors.grey.withValues(alpha: 0.3),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                  ),
                  child: const Text('Apply'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

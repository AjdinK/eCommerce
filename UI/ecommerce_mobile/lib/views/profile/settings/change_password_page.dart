import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_svg/svg.dart';
import 'package:provider/provider.dart';

import '../../../core/components/app_back_button.dart';
import '../../../core/constants/constants.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/user_provider.dart';
import '../../../utils/utils_widgets.dart';

class ChangePasswordPage extends StatefulWidget {
  const ChangePasswordPage({super.key});

  @override
  State<ChangePasswordPage> createState() => _ChangePasswordPageState();
}

class _ChangePasswordPageState extends State<ChangePasswordPage> {
 final _formKey = GlobalKey<FormBuilderState>();

  late UserProvider _userProvider = UserProvider();

  @override
  void initState() {
    super.initState();
    _userProvider = context.read<UserProvider>();
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: const AppBackButton(),
        title: const Text(
          'Change Password Page',
        ),
      ),
      backgroundColor: AppColors.cardColor,
      body: Center(
        child: SingleChildScrollView(
          child: Container(
            margin: const EdgeInsets.all(AppDefaults.padding),
            padding: const EdgeInsets.symmetric(
              horizontal: AppDefaults.padding,
              vertical: AppDefaults.padding * 2,
            ),
            decoration: BoxDecoration(
              color: AppColors.scaffoldBackground,
              borderRadius: AppDefaults.borderRadius,
            ),
            child: FormBuilder(
              key: _formKey,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  /* <----  Current Password -----> */
                  const Text("Current Password"),
                  const SizedBox(height: 8),
                  FormBuilderTextField(
                    name: 'password',
                    keyboardType: TextInputType.visiblePassword,
                    textInputAction: TextInputAction.next,
                    decoration: InputDecoration(
                      suffixIcon: Padding(
                        padding: const EdgeInsets.all(16),
                        child: SvgPicture.asset(AppIcons.eye),
                      ),
                      suffixIconConstraints: const BoxConstraints(),
                    ),
                     validator: (value) {
                        if (value == null || value.isEmpty) {
                          return mField;
                        } else {
                          return null;
                        }
                      },
                  ),
                  const SizedBox(height: AppDefaults.padding),
              
                  /* <---- New Password -----> */
                  const Text("New Password"),
                  const SizedBox(height: 8),
                  FormBuilderTextField(
                    name: 'newPassword',
                    keyboardType: TextInputType.visiblePassword,
                    textInputAction: TextInputAction.next,
                    decoration: InputDecoration(
                      suffixIcon: Padding(
                        padding: const EdgeInsets.all(16),
                        child: SvgPicture.asset(AppIcons.eye),
                      ),
                      suffixIconConstraints: const BoxConstraints(),
                    ),
                     validator: (value) {
                        if (value == null || value.isEmpty) {
                          return mField;
                        } else {
                          return null;
                        }
                      },
                  ),
                  const SizedBox(height: AppDefaults.padding),
              
                  /* <---- Confirm Password-----> */
                  const Text("Confirm Password"),
                  const SizedBox(height: 8),
                  FormBuilderTextField(
                    name: 'confirmPassword',
                    keyboardType: TextInputType.visiblePassword,
                    textInputAction: TextInputAction.done,
                    decoration: InputDecoration(
                      suffixIcon: Padding(
                        padding: const EdgeInsets.all(16),
                        child: SvgPicture.asset(AppIcons.eye),
                      ),
                      suffixIconConstraints: const BoxConstraints(),
                    ),
                    validator: (value) {
                        if (value == null || value.isEmpty) {
                          return mField;
                        } else if (value !=
                            _formKey.currentState?.value['newPassword']) {
                          return "New password doesn't match";
                        } else {
                          return null;
                        }
                      },
                  ),
                  const SizedBox(height: AppDefaults.padding),
              
                  /* <---- Submit -----> */
                  const SizedBox(height: AppDefaults.padding),
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      child: const Text('Update Password'),
                      onPressed: () async {
                         _formKey.currentState?.save();

                        try {
                          if (_formKey.currentState!.validate()) {
                            
                            await _userProvider.changePassword({
                              'id': AuthProvider.accessTokenDecoded?['Id'],
                              'password':
                                  _formKey.currentState?.value['password'],
                              'newPassword':
                                  _formKey.currentState?.value['newPassword'],
                              'confirmNewPassword': _formKey
                                  .currentState
                                  ?.value['confirmPassword'],
                            });

                             ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(
                                content: Text("Password successfully changed"),
                              ),
                            );
                            Navigator.pop(context);
                          }
                        } on Exception catch (e) {
                          alertBox(context, "Error", e.toString());
                        }
                      },
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

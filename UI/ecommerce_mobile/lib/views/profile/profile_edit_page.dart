import 'dart:convert';

import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../core/components/app_back_button.dart';
import '../../core/constants/constants.dart';
import '../../models/user.dart';
import '../../providers/user_provider.dart';
import '../../utils/utils_widgets.dart';

class ProfileEditPage extends StatefulWidget {
  final User user;
  const ProfileEditPage({super.key, required this.user});

  @override
  State<ProfileEditPage> createState() => _ProfileEditPageState();
}

class _ProfileEditPageState extends State<ProfileEditPage> {
  final _formKey = GlobalKey<FormBuilderState>();
  Map<String, dynamic> _initalValue = {};

  late UserProvider _userProvider;

  bool isLoading = true;

  String? base64ProfileImage;

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _userProvider = context.read<UserProvider>();

    base64ProfileImage = widget.user.profileImageBase64;

    _initalValue = {
      'firstName': widget.user.firstName,
      'lastName': widget.user.lastName,
      'email': widget.user.email,
      'username': widget.user.username,
      'phoneNumber': widget.user.phoneNumber,
    };

    _userProvider = context.read<UserProvider>();
  }

  Future _pickFile() async {
    try {
      FilePickerResult? result = await FilePicker.pickFiles(
        type: FileType.image,
      );

      if (result != null) {
        var file = result.files.first;
        var bytes = await file.xFile.readAsBytes();
        final base64String = base64Encode(bytes);

        setState(() {
          base64ProfileImage = base64String;
        });
      }
    } catch (e) {
      alertBox(context, "Error", e.toString());
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.cardColor,
      appBar: AppBar(
        leading: const AppBackButton(),
        title: const Text('Profile'),
      ),
      body: SingleChildScrollView(
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
            initialValue: _initalValue,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(
                  width: double.infinity,
                  child: InkWell(
                    onTap: _pickFile,
                    child: CircleAvatar(
                      backgroundImage: base64ProfileImage != null
                          ? ImageFromBase64StringWithoutDimnesions(
                              base64ProfileImage!,
                            )
                          : AssetImage("assets/images/no_profile.png"),
                      radius: 70,
                    ),
                  ),
                ),

                SizedBox(height: AppDefaults.padding * 2),
                /* <----  First Name -----> */
                const Text("First Name"),
                const SizedBox(height: 8),
                FormBuilderTextField(
                  name: 'firstName',
                  keyboardType: TextInputType.text,
                  textInputAction: TextInputAction.next,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return mField;
                    } else {
                      return null;
                    }
                  },
                ),
                const SizedBox(height: AppDefaults.padding),

                /* <---- Last Name -----> */
                const Text("Last Name"),
                const SizedBox(height: 8),
                FormBuilderTextField(
                  name: 'lastName',
                  keyboardType: TextInputType.text,
                  textInputAction: TextInputAction.next,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return mField;
                    } else {
                      return null;
                    }
                  },
                ),
                const SizedBox(height: AppDefaults.padding),

                /* <---- Phone Number -----> */
                const Text("Username"),
                const SizedBox(height: 8),
                FormBuilderTextField(
                  name: 'username',
                  keyboardType: TextInputType.text,
                  textInputAction: TextInputAction.next,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return mField;
                    } else {
                      return null;
                    }
                  },
                ),
                const SizedBox(height: AppDefaults.padding),

                /* <---- Gender -----> */
                const Text("Email"),
                const SizedBox(height: 8),
                FormBuilderTextField(
                  name: 'email',
                  keyboardType: TextInputType.emailAddress,
                  textInputAction: TextInputAction.next,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return mField;
                    } else if (!RegExp(
                      r"^[a-zA-Z0-9.a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9]+\.[a-zA-Z]+",
                    ).hasMatch(value)) {
                      return "Invalid email";
                    } else {
                      return null;
                    }
                  },
                ),
                const SizedBox(height: AppDefaults.padding),

                /* <---- Birthday -----> */
                const Text("Phone"),
                const SizedBox(height: 8),
                FormBuilderTextField(
                  name: 'phone',
                  keyboardType: TextInputType.text,
                  textInputAction: TextInputAction.next,
                ),
                const SizedBox(height: AppDefaults.padding),

                /* <---- Submit -----> */
                const SizedBox(height: AppDefaults.padding),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    child: const Text('Save'),
                    onPressed: () async {
                      _formKey.currentState?.save();

                      try {
                        if (_formKey.currentState!.validate()) {
                          Map<String, dynamic> request = Map.of(
                            _formKey.currentState!.value,
                          );

                          if (base64ProfileImage != null) {
                            request['profileImageBase64'] = base64ProfileImage;
                          }

                          await _userProvider.update(widget.user.id!, request);

                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text("Profile successfully edited"),
                            ),
                          );

                          Navigator.pop(context, 'reload');
                        }
                      } on Exception catch (e) {
                        alertBox(context, "Error", e.toString());
                      }
                      if (_formKey.currentState!.validate()) {
                        Map<String, dynamic> request = Map.of(
                          _formKey.currentState!.value,
                        );

                        await _userProvider.update(widget.user.id!, request);
                      }
                    },
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/components/asset_image.dart';
import '../../../core/components/base64_image.dart';
import '../../../core/constants/constants.dart';
import '../../../models/user.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/user_provider.dart';
import '../../../utils/utils_widgets.dart';
import 'profile_header_options.dart';

class ProfileHeader extends StatelessWidget {
  const ProfileHeader({super.key});

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        /// Background
        Image.asset('assets/images/profile_page_background.png'),

        /// Content
        Column(
          children: [
            AppBar(
              title: const Text('Profile'),
              elevation: 0,
              backgroundColor: Colors.transparent,
              titleTextStyle: Theme.of(context).textTheme.titleLarge?.copyWith(
                color: Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
            const _UserData(),
            const ProfileHeaderOptions(),
          ],
        ),
      ],
    );
  }
}

class _UserData extends StatefulWidget {
  const _UserData();

  @override
  State<_UserData> createState() => __UserDataState();
}

class __UserDataState extends State<_UserData> {
  late UserProvider _userProvider;

  late User user;

  bool isLoading = true;

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _userProvider = context.read<UserProvider>();

    initData();
  }

  Future<void> initData() async {
    try {
      var result = await _userProvider.getById(
        int.tryParse(AuthProvider.accessTokenDecoded?['Id'] ?? '0') ?? 0,
      );

      setState(() {
        user = result;
        isLoading = false;
      });
    } on Exception catch (e) {
      alertBox(context, 'Error', e.toString());
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(AppDefaults.padding),
      child: isLoading
          ? const CircularProgressIndicator()
          : Row(
        children: [
          const SizedBox(width: AppDefaults.padding),
          SizedBox(
            width: 100,
            height: 100,
            child: ClipOval(
              child: AspectRatio(
                aspectRatio: 1 / 1,
                child: user.profileImageBase64 != null
                    ? Base64ImageWithLoader(user.profileImageBase64 ?? '')
                    : AssetImageWithLoader(
                        'assets/images/no_profile.png',
                      ),
              ),
            ),
          ),
          const SizedBox(width: AppDefaults.padding),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                "${user.firstName} ${user.lastName}",
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Colors.white,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Username: ${user.username}',
                style: Theme.of(
                  context,
                ).textTheme.bodyLarge?.copyWith(color: Colors.white),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/constants/constants.dart';
import '../../../core/routes/app_routes.dart';
import '../../../models/user.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/user_provider.dart';
import '../../../utils/utils_widgets.dart';
import 'profile_list_tile.dart';

class ProfileMenuOptions extends StatefulWidget {
  const ProfileMenuOptions({super.key});

  @override
  State<ProfileMenuOptions> createState() => _ProfileMenuOptionsState();
}

class _ProfileMenuOptionsState extends State<ProfileMenuOptions> {
 late UserProvider _userProvider;

  late User user;

  bool isLoading = true;

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _userProvider = context.read<UserProvider>();

    initUser();
  }

  Future<void> initUser() async {
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
    return Container(
      margin: const EdgeInsets.all(AppDefaults.padding),
      padding: const EdgeInsets.all(AppDefaults.padding),
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: AppDefaults.boxShadow,
        borderRadius: AppDefaults.borderRadius,
      ),
      child: Column(
        children: [
          isLoading ? const CircularProgressIndicator() : ProfileListTile(
            title: 'My Profile',
            icon: AppIcons.profilePerson,
            onTap: () async { 
              var refresh = await Navigator.pushNamed(context, AppRoutes.profileEdit, arguments: user);

              if(refresh == 'reload') {
                  initUser();
              }
            },
          ),
          const Divider(thickness: 0.1),
          ProfileListTile(
            title: 'Notification',
            icon: AppIcons.profileNotification,
            onTap: () => Navigator.pushNamed(context, AppRoutes.notifications),
          ),
          const Divider(thickness: 0.1),
          ProfileListTile(
            title: 'Setting',
            icon: AppIcons.profileSetting,
            onTap: () => Navigator.pushNamed(context, AppRoutes.settings),
          ),
          const Divider(thickness: 0.1),
          ProfileListTile(
            title: 'Payment',
            icon: AppIcons.profilePayment,
            onTap: () => Navigator.pushNamed(context, AppRoutes.paymentMethod),
          ),
          const Divider(thickness: 0.1),
          ProfileListTile(
            title: 'Logout',
            icon: AppIcons.profileLogout,
            onTap: () async {
              final leave = await showDialog<bool>(
                context: context,
                builder: (ctx) => AlertDialog(
                  title: Text("Log out"),
                  content: Text("Are you sure you want to log out?"),
                  actions: [
                    TextButton(
                      onPressed: () => Navigator.pop(ctx, false),
                      child: Text("Cancel"),
                    ),
                    TextButton(
                      onPressed: () => Navigator.pop(ctx, true),
                      child: Text("Log out"),
                    ),
                  ],
                ),
              );
              if (leave != true || !mounted) return;
              context.read<AuthProvider>().logout();
              if (!mounted) return;
              Navigator.of(context).pushNamedAndRemoveUntil(
                AppRoutes.login,
                (route) => route.isFirst,
              );
            },
          ),
        ],
      ),
    );
  }
}

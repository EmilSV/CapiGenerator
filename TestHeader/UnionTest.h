union test {
  int button;
  char char_button;
};


typedef struct SDL_GamepadBinding
{
    int input_type;
    union
    {
        int button;

        struct
        {
            int axis;
            int axis_min;
            int axis_max;
        } axis;

        struct
        {
            int hat;
            int hat_mask;
        } hat;

    } input;

    int output_type;
    union
    {
        int button;

        struct
        {
            int axis;
            int axis_min;
            int axis_max;
        } axis;

    } output;
} SDL_GamepadBinding;

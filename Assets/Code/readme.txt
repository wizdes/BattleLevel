Create initial movements:
private void CreateInitialMovement()
{
    if (starto == false)
    {
        List<Tuple<int, int>> movements = new List<Tuple<int, int>>();
        Tuple<int, int> x = new Tuple<int, int>(0, 1);
        movements.Add(x);
        x = new Tuple<int, int>(1, 0);
        movements.Add(x);
        Move(movements);
        starto = true;
    }
}

